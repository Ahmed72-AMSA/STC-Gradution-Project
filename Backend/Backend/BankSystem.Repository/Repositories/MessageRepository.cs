using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Repository.Repositories
{
    using global::BankSystem.Data.Contexts;
    using global::BankSystem.Data.Entities;
    using global::BankSystem.Repository.RepositoryInterfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    namespace BankSystem.Data.Repositories
    {


        public class MessageRepository : IMessageRepository
        {
            private readonly BankingContext _context;

            public MessageRepository(BankingContext context)
            {
                _context = context;
            }

            public async Task AddMessageAsync(Messages message)
            {
                await _context.Messages.AddAsync(message);
                await _context.SaveChangesAsync();
            }

            public async Task<List<Messages>> GetConversationAsync(string user1, string user2)
            {
                return await _context.Messages
                    .Where(m => (m.SenderUsername == user1 && m.ReceiverUsername == user2) ||
                               (m.SenderUsername == user2 && m.ReceiverUsername == user1))
                    .OrderBy(m => m.SentAt)
                    .ToListAsync();
            }

            public async Task<List<string>> GetUserConversationsAsync(string username)
            {
                return await _context.Messages
                    .Where(m => m.SenderUsername == username || m.ReceiverUsername == username)
                    .Select(m => m.SenderUsername == username ? m.ReceiverUsername : m.SenderUsername)
                    .Distinct()
                    .ToListAsync();
            }
        }
    }
}
