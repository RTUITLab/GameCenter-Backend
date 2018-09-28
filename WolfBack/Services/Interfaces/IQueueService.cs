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
        List<Score> GetFromQueue(int count);
        Score FindInQueue(Score Player);
        int GetCount();
    }
}
