using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BenihanaWebReact.Hubs
{
    public class ChatHub: Hub
    {
        public ChatHub()
        {

        }

        public async Task SendConnectionId(string connectionId)
        {
            await Clients.All.SendAsync("setClientMessage", "A connection with ID '" + connectionId + "' has just connected");
        }

        public async Task SendSimulationResult(string connectionId)
        {
            await Clients.Client(connectionId).SendAsync("You should receive your simulation run");
        }
    }
}
