using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WolfBack.Services.Interfaces;
using Models;

namespace WolfBack.Services
{
    public class QueueService : IQueueService
    {

        public List<Score> list = new List<Score>();
        public void PutInQueue(Score player)
        {
            list.Add(player);
        }

        public void DeletePlayer(Score player)
        {
            list.RemoveAll(p => p.PlayerName.Username == player.PlayerName.Username);
        }

        public void DeletePlayers(GameType player)
        {
            list.RemoveAll(p => p.GameTypeId == player.GameTypeId);
        }
        public List<Score> GetQueue(int count)
        {
            return list;
        }
        public Score FindInQueue(Guid player)
        {
            return list.FirstOrDefault(id => id.PlayerId == player);
        }
        public int GetCount()
        {
            return list.Count;
        }
    }
}
