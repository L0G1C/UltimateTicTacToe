using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CSharpVitamins;
using UltimateTicTacToe.Web.Models;

namespace UltimateTicTacToe.Web.Logic
{
    public class GameManager : IGameManager
    {
        private static readonly ConcurrentBag<Game> _games = new ConcurrentBag<Game>();        

        public GameState GetGameState(string connectionId)
        {
            var existingGame = _games.FirstOrDefault(g => g.PlayerA.ConnectionId == connectionId);
            if (existingGame == null)
            {
                return GameState.NewGame;
            }
            
            if(existingGame.MoveHistory.Count < 81)
            {
                if (existingGame.MoveHistory.LastOrDefault().Key == existingGame.PlayerA.ConnectionId)
                {
                    return GameState.PlayerA;
                }
                if (existingGame.MoveHistory.LastOrDefault().Key == existingGame.PlayerB.ConnectionId)
                {
                    return GameState.PlayerB;
                }
            }

            return GameState.GameOver;

        }

        public string CreateGame(string name, string connectionId)
        {
            ShortGuid newGameId = Guid.NewGuid();

            try
            {
                _games.Add(new Game()
                {
                    PlayerA = new Player()
                    {
                        Name = name,
                        ConnectionId = connectionId
                    },
                    GameId = newGameId
                });
            }
            catch(Exception ex) { }

            return newGameId.ToString();
        }

        public Game GetGame(string gameId)
        {
            if(!string.IsNullOrEmpty(gameId))
                return _games.FirstOrDefault(g => g.GameId == gameId);
            else 
                return new Game();
        }

        public void SetActivePlayer(string playerConnectionId, string gameId)
        {
            var game = _games.FirstOrDefault(g => g.GameId == gameId);

            game.ActivePlayer = playerConnectionId;
        }
    }
}
