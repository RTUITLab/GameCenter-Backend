using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WolfBack.Services.Interfaces
{
    public interface IQueueService
    {
        void PutInQueue(Player player);
        void DeletePlayer(Guid playerId);
        void DeletePlayers(Guid gameId);
        List<Player> GetQueue(int count);
        Player FindInQueue(Guid playerId);
        Player GetFirstInGameQueue(Guid gameId);
        int GetCount();
    }
}
