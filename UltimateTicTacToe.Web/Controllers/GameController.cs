using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace UltimateTicTacToe.Web.Controllers
{    
    public class GameController : Controller
    {
        public IActionResult Index(int gameId)
        {
            return View();
        }
    }
}