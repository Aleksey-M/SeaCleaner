using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeaCleaner.Server.Data;
using SeaCleaner.Server.Model;
using SeaCleaner.Shared;
using System;
using System.Threading.Tasks;

namespace SeaCleaner.Server.Controllers
{
    [ApiController]
    public class AccauntController : ControllerBase
    {
        private static GameDbContext _context;
        public AccauntController(GameDbContext context)
        {
            _context = context;
        }

        [HttpPost, Route("api/register")]
        public async Task<ActionResult<LoginActionResult>> PostGameRegister(RegisterDto registerDto)
        {
            var resp = new LoginActionResult();

            bool loginExists = await _context.Gamers.AnyAsync(g => g.Login == registerDto.Login);
            if (loginExists)
            {
                resp.Message = $@"Login '{registerDto.Login}' already exist";
                return resp;
            }

            if (!registerDto.Password.Equals(registerDto.ConfirmPasword, StringComparison.Ordinal))
            {
                resp.Message = "Password and Confirm Password values are not the same";
                return resp;
            }

            var token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();

            var newGamer = new Gamer
            {
                Id = Guid.NewGuid(),
                Login = registerDto.Login,
                PasswordHash = PasswordHelper.GetHash(registerDto.Password)
            };

            await _context.Gamers.AddAsync(newGamer);
            await _context.SaveChangesAsync();

            resp.Gamer = new GamerDto
            {
                Id = newGamer.Id,
                Login = newGamer.Login
            };

            return resp;
        }

        [HttpPost, Route("api/login")]
        public async Task<ActionResult<LoginActionResult>> GameLogin(LogInDto loginDto)
        {
            var resp = new LoginActionResult();
            var p = PasswordHelper.GetHash(loginDto.Password);

            var gamer = await _context.Gamers.SingleOrDefaultAsync(g => g.Login == loginDto.Login && g.PasswordHash == p);

            if (gamer == null)
            {
                resp.Message = "Incorrect Login or Password";
                return resp;
            }

            resp.Gamer = new GamerDto
            {
                Id = gamer.Id,
                Login = gamer.Login
            };

            return resp;
        }
    }
}
