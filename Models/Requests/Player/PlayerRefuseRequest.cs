using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Requests.Player
{
    public class PlayerRefuseRequest
    {
        public Guid PlayerId { get; set; }
        public Guid GameId { get; set; }
        public int Score { get; set; }
    }
}
