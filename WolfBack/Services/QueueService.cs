using System;
using System.Collections.Generic;
using System.Linq;
using WolfBack.Services.Interfaces;
using Models;

namespace WolfBack.Services
{
    public class QueueService : IQueueService
    {
        public List<Player> list = new List<Player>();

        public void PutInQueue(Player player)
        {
            list.Add(player);
        }
        public void DeletePlayer(Guid playerId)
        {
            list.RemoveAll(p => p.PlayerId == playerId);
        }
        public void DeletePlayers(Guid gameId)
        {
            list.RemoveAll(p => p.Score.GameTypeId == gameId);
        }
        public List<Player> GetQueue(int count)
        {
            return list;
        }
        public Player FindInQueue(Guid player)
        {
            return list.FirstOrDefault(id => id.PlayerId == player);
        }
        public Player GetFirstInGameQueue(Guid gameId)
        {
            return list.FirstOrDefault(id => id.Score.GameTypeId == gameId);
        }
        public int GetCount()
        {
            return list.Count;
        }
    }
}
