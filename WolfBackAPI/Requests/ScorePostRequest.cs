using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolfBackAPI.Requests
{
    public class ScorePostRequest
    {
        public Guid GameId { get; set; }
        public int Score { get; set; }
    }
}
