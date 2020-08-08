using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaCleaner.Server.Data;
using SeaCleaner.Server.Model;
using SeaCleaner.Shared;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SeaCleaner.Controllers
{
    public class GameController : ControllerBase
    {
        private static GameDbContext _context;
        public GameController(GameDbContext dataContext)
        {
            _context = dataContext;
        }

        [HttpPost, Route("api/win")]
        public async Task<ActionResult> Win(AddGameResultDto results)
        {
            var gamer = await _context.Gamers.FindAsync(results.GamerId);

            if (gamer == null)
            {
                return BadRequest($@"Gamer with Id='{results.GamerId}' is not exists");
            }

            await _context.GameResults.AddAsync(new GameResult
            {
                Id = Guid.NewGuid(),
                GamerId = gamer.Id,
                GameDate = DateTimeOffset.Now,
                IsVictory = results.SavedDolphins > 0,
                SavedDolphins = results.SavedDolphins,
                GameDuration = results.Seconds
            });

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet, Route("api/rating")]
        public async Task<RatingTable> GetRating()
        {
            var ratingTable = new RatingTable
            {
                ResultsList = await _context.GameResults
                    .OrderByDescending(gr => gr.SavedDolphins)
                    .ThenBy(gr => gr.GameDuration)
                    .Take(10)
                    .Select(gr => new GameResultsRow
                    {
                        GamerName = gr.Gamer.Login,
                        GamerId = gr.GamerId,
                        SavedDolphins = gr.SavedDolphins,
                        GameDuration = gr.GameDuration,
                        GameDate = gr.GameDate
                    })
                    .ToListAsync()
            };

            return ratingTable;
        }
    }
}
