using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UltimateTicTacToe.Web.Models;

namespace UltimateTicTacToe.Web.Logic
{
    public class GameManager : IGameManager
    {
        private readonly IEnumerable<Game> _games;

        public GameState GetGameState(string contextConnectionId)
        {
            if (_games.FirstOrDefault(g => g.MoveHistory.ContainsKey(contextConnectionId)) == null)
            {
                return GameState.NewGame;
            }

            if (_games.FirstOrDefault(g => g.MoveHistory.LastOrDefault() == )
            {
                return GameState.NewGame;
            }
        } 
    }
}
