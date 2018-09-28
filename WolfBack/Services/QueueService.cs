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
        public ConcurrentQueue<Score> queue = new ConcurrentQueue<Score>();
        public List<Score> list = new List<Score>();
        public void PutInQueue(Score player)
        {
            queue.Enqueue(player);
        }
        public List<Score> GetFromQueue(int count)
        {
            for (int i = 0; i < count; i++)
            {
                queue.TryDequeue(out Score result);
                list.Add(result);
            }
            return list;
        }
        public Score FindInQueue(Score player)
        {
            return queue.FirstOrDefault(id => id.PlayerId == player.PlayerId);
        }
        public int GetCount()
        {
            return queue.Count;
        }
    }
}
