using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WolfBack.Services.Interfaces;
using WolfBack.Extencions;

namespace WolfBack.Services
{
    public class QueueService : IQueueService
    {
        public ConcurrentQueue<Guid> queue = new ConcurrentQueue<Guid>();

        public void PutInQueue(Guid playerId)
        {
            queue.Enqueue(playerId);
        }
        public List<Guid> GetFromQueue(int count)
        {
            return queue.Take(count);
        }
    }
}
