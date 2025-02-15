using STC.Data;
using STC.Models.Chat_Service;
using Microsoft.AspNetCore.SignalR;
using STC.Data.Context;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using STC.Services.ChatServices;
using System.Linq;
using STC.Models;

namespace BankingSystemAPI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly STCSystemDbContext _context;
        private readonly ILogger<ChatHub> _logger;
        private readonly SharedDb _shared;

        public ChatHub(STCSystemDbContext context, ILogger<ChatHub> logger, SharedDb shared)
        {
            _context = context;
            _logger = logger;
            _shared = shared;
        }

        // Method to handle user connection
        public async Task ConnectUser(string username)
        {
            var connection = new SignUp
            {
                UserName = username, 
                ConnectionId = Context.ConnectionId
            };

            // Add to shared database for tracking connections
            _shared.Connections[username] = connection;

            // Notify that the user has connected
            await Clients.All.SendAsync("UserConnected", username);
            _logger.LogInformation($"{username} connected with ConnectionId: {Context.ConnectionId}");
        }

        // Method to send a message (private one-to-one chat only)
        public async Task SendPrivateMessage(string recipient, string sender, string message)
        {
            var senderUser = _context.Users.FirstOrDefault(u => u.UserName == sender);
            var recipientUser = _context.Users.FirstOrDefault(u => u.UserName == recipient);

            if (senderUser == null || recipientUser == null)
            {
                await Clients.Caller.SendAsync("Error", "User not found.");
                return;
            }

            // Validate sender and recipient roles
            if (senderUser.Role == "User" && recipientUser.Role != "CustomerService")
            {
                await Clients.Caller.SendAsync("Error", "You can only send messages to CustomerService.");
                return;
            }

            if (senderUser.Role == "CustomerService")
            {
                // CustomerService can send to any user
                await Clients.User(recipient).SendAsync("ReceivePrivateMessage", sender, message);
            }

            // Save the message in the database for permanent storage
            var msg = new Message
            {
                Sender = sender,
                Recipient = recipient,
                Text = message,
                Timestamp = DateTime.Now,
                IsRead = false // Message starts as unread
            };
            _context.Messages.Add(msg);
            await _context.SaveChangesAsync();
        }

        // Method to show the messages for a specific user
        public async Task GetUserMessages(string username)
        {
            var messages = _context.Messages
                .Where(m => m.Sender == username || m.Recipient == username)
                .OrderBy(m => m.Timestamp) // Ensure messages are ordered by timestamp
                .ToList();

            await Clients.Caller.SendAsync("ShowMessages", messages);
        }

        // Method to handle user disconnection
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connection = _shared.Connections.Values.FirstOrDefault(c => c.ConnectionId == Context.ConnectionId);
            if (connection != null)
            {
                // Remove the connection from the shared dictionary
                _shared.Connections.TryRemove(connection.UserName, out _);

                // Notify all clients about the disconnection
                await Clients.All.SendAsync("UserDisconnected", connection.UserName);
                _logger.LogInformation($"{connection.UserName} disconnected");
            }

            await base.OnDisconnectedAsync(exception);
        }
    }
}
