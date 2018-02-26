using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UltimateTicTacToe.Web.Models;

namespace UltimateTicTacToe.Web.Controllers
{    
    public class GameController : Controller
    {
        public IActionResult Index(int gameId)
        {
            ViewBag.PlayerName = "";

            return View();
        }

        [HttpPost]
        public IActionResult Index(Player player)
        {
            ViewBag.PlayerName = player.Name;

            return View();
        }
    }
}