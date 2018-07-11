using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class GameType
    {
        public Guid Id { get; set; }
        public string GameName { get; set; }
        public List<Score> Scores { get; set; }
    }
}
