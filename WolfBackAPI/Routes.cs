using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WolfBackAPI
{
    public static class Routes
    {
        public static readonly string PostScore = "WolfBackAPI/score";
        /// <summary>
        /// WolfBackAPI/game/ + gamename
        /// </summary>
        public static readonly string GetGameId = "WolfBackAPI/game/";
    }
}
