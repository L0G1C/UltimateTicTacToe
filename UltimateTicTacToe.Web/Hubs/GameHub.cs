using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using UltimateTicTacToe.Web.Logic;

namespace UltimateTicTacToe.Web.Hubs
{
    
    public class GameHub : Hub
    {
        private readonly IGameManager _gameManager;

        public GameHub(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.Client(Context.ConnectionId).InvokeAsync("consoleLog", "Connection Established!!");

            var gameState = _gameManager.GetGameState(Context.ConnectionId);

            if (gameState == GameState.NewGame)
            {
                await Clients.Client(Context.ConnectionId).InvokeAsync("consoleLog", "Starting a new game");
                await Clients.Client(Context.ConnectionId).InvokeAsync("newGameInit");               
            }

            await base.OnConnectedAsync();
        }

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.InvokeAsync("broadcastMessage", name, message);
        }

        public void NewGame(string name)
        {
            var gameId =_gameManager.CreateGame(name, Context.ConnectionId);

            Clients.All.InvokeAsync("newGameComplete", gameId);
        }
    }
}
