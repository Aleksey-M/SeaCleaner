using System;

namespace SeaCleaner.Domain
{
    public class GameResults
    {
        public int GameResultsId { get; set; }
        public DateTimeOffset GameDate { get; set; }
        public TimeSpan GameTime { get; set; }
        public int Dolphins { get; set; }

        public Player Player { get; set; }
    }
}
