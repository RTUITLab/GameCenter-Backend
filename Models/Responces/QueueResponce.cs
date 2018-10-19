using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responces
{
    public class QueueResponce
    {
        public Guid GameId { get; set; }
        public Guid PlayerId { get; set; }
        public string GameName { get; set; }
        public string UserName { get; set; }
    }
}
