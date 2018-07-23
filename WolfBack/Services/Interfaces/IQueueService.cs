using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WolfBack.Services.Interfaces
{
    public interface IQueueService
    {
        void PutInQueue(Guid playerId);
        List<Guid> GetFromQueue(int count);
    }
}
