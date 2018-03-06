using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpVitamins;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using UltimateTicTacToe.Web.Logic;
using UltimateTicTacToe.Web.Models;

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
            await Clients.Client(Context.ConnectionId).InvokeAsync("consoleLog", "Connection Established!!", Context.ConnectionId);

            var gameState = _gameManager.GetGameState(Context.ConnectionId);

            if (gameState == GameState.NewGame)
            {
                await Clients.Client(Context.ConnectionId).InvokeAsync("consoleLog", "Starting a new game");
                await Clients.Client(Context.ConnectionId).InvokeAsync("newGameInit");               
            }
            else
            {                
                // 
            }

            await base.OnConnectedAsync();
        }

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.InvokeAsync("broadcastMessage", name, message);
        }

        public async Task NewGame(string player)
        {
            var playerobj = JsonConvert.DeserializeObject<Player>(player);

            var gameId =_gameManager.CreateGame(playerobj.Name, playerobj.ConnectionId);

            await Groups.AddAsync(Context.ConnectionId, gameId);

            //var url = "http://localhost:65350";
            var url = "http://ultimatetictactoeweb20180226014509.azurewebsites.net";
            await  Clients.Group(gameId).InvokeAsync("newGameComplete", gameId, url);
        }

        public async Task AddPlayer(string player, string gameId)
        {
            // User is connecting to Existing game. Add the Player Obj to the existing game            
            var playerObj = JsonConvert.DeserializeObject<Player>(player);
            var gameIdObj = JsonConvert.DeserializeObject<ShortGuid>(gameId);
            var existingGame = _gameManager.GetGame(gameIdObj);
            existingGame.PlayerB = playerObj;

            await Groups.AddAsync(Context.ConnectionId, gameIdObj.Value);

            // Choose an active player and begin the game!
            Random rand = new Random();
            var activePlayer = rand.Next(2);
            if (activePlayer == 0)
                _gameManager.SetActivePlayer(existingGame.PlayerA.ConnectionId, existingGame.GameId);
            if (activePlayer == 1)
                _gameManager.SetActivePlayer(existingGame.PlayerB.ConnectionId, existingGame.GameId);

            await Clients.Group(gameIdObj.Value).InvokeAsync("playerTurn", existingGame);
        }
    }
}
