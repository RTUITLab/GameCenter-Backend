using System;
using System.Collections.Generic;

namespace Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public List<Score> Scores { get; set; }
    }
}
