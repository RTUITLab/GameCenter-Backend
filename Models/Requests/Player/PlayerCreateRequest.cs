using System;
using System.Collections.Generic;
using System.Text;

namespace Models.ModelViews
{
    public class PlayerCreateRequest
    {
        public Guid GameId { get; set; }
        public string Username { get; set; }
    }
}
