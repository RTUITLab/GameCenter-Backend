using System;
using System.Collections.Generic;

namespace Models
{
    public class Player
    {
        public Guid PlayerId { get; set; }
        public string Username { get; set; }
        public string VKId { get; set; }
        public List<Score> Scores { get; set; }
        public PlayerStatus Status { get; set; }
    }
}
