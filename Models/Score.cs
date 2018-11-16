using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Score
    {
        public Guid ScoreId { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameTypeId { get; set; }
        public Player Player { get; set; }
        public GameType GameType { get; set; }
        public int ScoreCount { get; set; }
        public DateTime Date { get; set; }
    }
}
