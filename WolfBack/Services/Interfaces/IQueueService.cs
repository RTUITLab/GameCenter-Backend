using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WolfBack.Services.Interfaces
{
    public interface IQueueService
    {
        void PutInQueue(Score player);
        void DeletePlayer(Score player);
        void DeletePlayers(GameType player);
        List<Score> GetQueue(int count);
        Score FindInQueue(string Player);
        int GetCount();
    }
}
