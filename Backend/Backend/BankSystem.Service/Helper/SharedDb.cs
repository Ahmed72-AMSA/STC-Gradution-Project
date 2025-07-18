using BankSystem.Data.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Helper
{
    public class SharedDb
    {
        private readonly ConcurrentDictionary<string, User> _connections = new ConcurrentDictionary<string, User>();

        public ConcurrentDictionary<string, User> Connections => _connections;
    }
}
