using System.Collections.Concurrent;
using STC.Models; // Ensure SignUp model is properly referenced

namespace STC.Services.ChatServices
{
    public class SharedDb
    {
        // Using SignUp as the value type in the ConcurrentDictionary
        private readonly ConcurrentDictionary<string, SignUp> _connections = new ConcurrentDictionary<string, SignUp>();

        // Expose the dictionary as a public property for accessing connections
        public ConcurrentDictionary<string, SignUp> Connections => _connections;
    }
}
