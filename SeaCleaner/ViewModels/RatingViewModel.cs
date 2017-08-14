using SeaCleaner.Domain;
using System;
using System.Collections.Generic;

namespace SeaCleaner.ViewModels
{
    public class GameResultsRec
    {
        public string UserName { get; set; }
        public DateTimeOffset GameDate { get; set; }
        public TimeSpan GameTime { get; set; }
        public int Dolphins { get; set; }
    }

    public class RatingViewModel
    {
        public List<GameResultsRec> ResultsList { get; set; }
    }
}
