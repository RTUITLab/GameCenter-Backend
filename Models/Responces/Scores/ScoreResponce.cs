using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Responces.Scores
{
    public class ScoreResponce
    {
        public Guid ScoreId { get; set; }
        public string UserName { get; set; }
        public string GameName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }
}
