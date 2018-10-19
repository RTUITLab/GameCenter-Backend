using System;

namespace Models
{
    public class Player
    {
        public Guid PlayerId { get; set; }
        public string Username { get; set; }
        public Score Score { get; set; }
        public PlayerStatus Status { get; set; }
    }
}
