using System;
using System.Collections.Generic;

namespace SeaCleaner.Server.Model
{
    public class Gamer
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }

        public List<GameResult> GameResults { get; set; }
    }

    public class GameResult
    {
        public Guid Id { get; set; }
        public DateTimeOffset GameDate { get; set; }
        public long GameDuration { get; set; }
        public int SavedDolphins { get; set; }
        public bool IsVictory { get; set; }

        public Guid GamerId { get; set; }
        public Gamer Gamer { get; set; }
    }
}
