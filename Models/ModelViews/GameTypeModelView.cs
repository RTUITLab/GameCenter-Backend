using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ModelViews
{
    public class GameTypeModelView
    {
        public string GameType { get; set; }
        public Guid GameId { get; set; }
        public GameState State { get; set; }
    }
}
