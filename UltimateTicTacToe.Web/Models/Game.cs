using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpVitamins;

namespace UltimateTicTacToe.Web.Models
{
    public class Game
    {
        public ShortGuid GameId;
        public Dictionary<string, string> MoveHistory;
        public Player PlayerA;
        public Player PlayerB;        
    }
}
