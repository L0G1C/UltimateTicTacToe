using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UltimateTicTacToe.Web.Logic;
using UltimateTicTacToe.Web.Models;

namespace UltimateTicTacToe.Web.Controllers
{    
    public class GameController : Controller
    {
        private readonly IGameManager _gameManager;

        public GameController(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

 
        public IActionResult Index(string id)
        {
            ViewBag.PlayerName = "";
            var existingGame = _gameManager.GetGame(id);
            if (existingGame != null)
            {
                ViewBag.Game = JsonConvert.SerializeObject(existingGame);

            }
            return View(existingGame);

        }

        [HttpPost]
        public IActionResult Index(string playerObjectString, string gameObjectString, string name)
        {            
            var gameObject = JsonConvert.DeserializeObject<Game>(gameObjectString);

            ViewBag.PlayerName = name;          

            if (gameObject.GameId.Value != null)
            {
                var existingGame = _gameManager.GetGame(gameObject.GameId);
                
                return View(existingGame);
            }

            return View();
        }
    }
}