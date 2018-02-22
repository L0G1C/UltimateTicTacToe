using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace UltimateTicTacToe.Web.Hubs
{
    
    public class GameHub : Hub
    {

        public override async Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId).InvokeAsync("connectionmade", "connection made dude");

            await base.OnConnectedAsync();
        }

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.InvokeAsync("broadcastMessage", name, message);
        }
    }
}
