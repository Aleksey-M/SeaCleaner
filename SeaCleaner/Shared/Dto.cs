using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SeaCleaner.Shared
{    
    public class GamerDto
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
    }

    public class LoginActionResult
    {
        public GamerDto Gamer { get; set; }
        public string Message { get; set; }
    }

    public class LogInDto
    {
        [Required, StringLength(20)]
        public string Login { get; set; }
        [Required, StringLength(20)]
        public string Password { get; set; }

        public void Clear()
        {
            Login = string.Empty;
            Password = string.Empty;
        }
    }

    public class RegisterDto
    {
        [Required, StringLength(20)]
        public string Login { get; set; }
        [Required, StringLength(20)]
        public string Password { get; set; }
        [Required, StringLength(20)]
        public string ConfirmPasword { get; set; }

        public void Clear()
        {
            Login = string.Empty;
            Password = string.Empty;
            ConfirmPasword = string.Empty;
        }
    }

    public class AddGameResultDto
    {
        public Guid GamerId { get; set; }
        public bool Victory { get; set; }
        public int SavedDolphins { get; set; }
        public long Seconds { get; set; }
    }

    public class GameResultsRow
    {
        public string GamerName { get; set; }
        public Guid GamerId { get; set; }
        public DateTimeOffset GameDate { get; set; }
        public long GameDuration { get; set; }
        public int SavedDolphins { get; set; }
    }

    public class RatingTable
    {
        public List<GameResultsRow> ResultsList { get; set; }
    }
}
