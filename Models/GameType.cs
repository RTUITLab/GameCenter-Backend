using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class GameType
    {
        public Guid GameTypeId { get; set; }
        public string GameName { get; set; }
        public string ImageURL { get; set; }
        public List<Score> Scores { get; set; }
        public GameState State { get; set; }
    }
}
