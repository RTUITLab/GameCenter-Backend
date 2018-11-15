using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responces
{
    public class GameTypeResponse
    {
        public Guid GameId { get; set; }
        public string GameName { get; set; }
        public string ImageURL { get; set; }
    }
}
