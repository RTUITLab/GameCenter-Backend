using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Score
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public Guid GameTypeId { get; set; }
        public Player PlayerName { get; set; }
        public GameType GameType { get; set; }
        public int ScoreCount { get; set; }
    }
}
