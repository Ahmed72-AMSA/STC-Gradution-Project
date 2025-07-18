using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BankSystem.Data.Entities;
using Twilio.TwiML.Messaging;
using BankSystem.Data.Contexts;

public class ChatHub : Hub
{
    private readonly BankingContext _dbContext;
    private static readonly Dictionary<string, string> ConnectedUsers = new();

    public ChatHub(BankingContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ConnectUser(string username)
    {
        // Handle reconnection - update connection ID if user exists
        if (ConnectedUsers.ContainsKey(username))
        {
            ConnectedUsers[username] = Context.ConnectionId;
        }
        else
        {
            ConnectedUsers.Add(username, Context.ConnectionId);
            await Clients.All.SendAsync("UpdateUserList", ConnectedUsers.Keys.ToList());
        }

        // Mark user's unread messages as read
        await MarkMessagesAsRead(username);
    }

    public async Task GetConnectedUsers()
    {
        await Clients.Caller.SendAsync("UpdateUserList", ConnectedUsers.Keys.ToList());
    }

    public async Task SendPrivateMessage(string targetUser, string senderUser, string message)
    {
        if (ConnectedUsers.TryGetValue(targetUser, out var connectionId))
        {
            // Save message to database
            var newMessage = new Messages
            {
                SenderUsername = senderUser,
                ReceiverUsername = targetUser,
                Content = message,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _dbContext.Messages.Add(newMessage);
            await _dbContext.SaveChangesAsync();

            // Send message to recipient
            await Clients.Client(connectionId).SendAsync("ReceivePrivateMessage", senderUser, message);

            // Also send to sender for their own chat history
            await Clients.Caller.SendAsync("ReceivePrivateMessage", senderUser, message);
        }
        else
        {
            await Clients.Caller.SendAsync("UserNotFound", targetUser);
        }
    }

    public async Task GetMessageHistory(string user1, string user2)
    {
        var messages = await _dbContext.Messages
            .Where(m => (m.SenderUsername == user1 && m.ReceiverUsername == user2) ||
                        (m.SenderUsername == user2 && m.ReceiverUsername == user1))
            .OrderBy(m => m.SentAt)
            .ToListAsync();

        await Clients.Caller.SendAsync("LoadMessageHistory", messages);
    }

    private async Task MarkMessagesAsRead(string username)
    {
        var unreadMessages = await _dbContext.Messages
            .Where(m => m.ReceiverUsername == username && !m.IsRead)
            .ToListAsync();

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }

        if (unreadMessages.Any())
        {
            await _dbContext.SaveChangesAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var item = ConnectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (!string.IsNullOrEmpty(item.Key))
        {
            ConnectedUsers.Remove(item.Key);
            await Clients.All.SendAsync("UpdateUserList", ConnectedUsers.Keys.ToList());
        }
        await base.OnDisconnectedAsync(exception);
    }
}