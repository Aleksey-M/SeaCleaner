using Microsoft.AspNetCore.Mvc;
using SeaCleaner.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SeaCleaner.ViewModels;
using System;
using System.Linq;
using SeaCleaner.Persistance;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SeaCleaner.Controllers
{
    public class GameController : Controller
    {
        private static GameDataContext _dataContext;
        private static ILogger<GameController> _logger;
        public GameController(GameDataContext dataContext, ILogger<GameController> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        [Authorize]
        public IActionResult Play()
        {
            return View();
        }
                
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Win(GameResultsViewModel results)
        {            
            if (results.Victory)
            {
                var gameRes = new GameResults
                {
                    Dolphins = results.Dolphins,
                    GameDate = DateTimeOffset.Now,
                    GameTime = TimeSpan.FromSeconds(results.Seconds),
                    Player = await _dataContext.Users.FirstAsync(u=>u.UserName != null && u.UserName.Equals(User.Identity.Name))
                };
                _dataContext.GameResults.Add(gameRes);
                await _dataContext.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
