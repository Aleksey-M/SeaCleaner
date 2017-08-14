using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeaCleaner.ViewModels;
using SeaCleaner.Persistance;
using Microsoft.EntityFrameworkCore;

namespace SeaCleaner.Controllers
{
    public class HomeController : Controller
    {
        private readonly GameDataContext _context;
        public HomeController(GameDataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var top = _context.GameResults
                .OrderByDescending(gr => gr.Dolphins)
                .ThenBy(gr => gr.GameTime)
                .Take(10)
                .Select(gr => new GameResultsRec { UserName = gr.Player.UserName, Dolphins = gr.Dolphins, GameTime = gr.GameTime, GameDate = gr.GameDate })
                .ToList();

            return View(new RatingViewModel { ResultsList = top});
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
