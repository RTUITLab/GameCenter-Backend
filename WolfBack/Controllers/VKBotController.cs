using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.ModelViews;
using WolfBack.Services.Interfaces;
using WolfBack.SignalR;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/VkBot")]
    public class VKBotController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IQueueService queue;

        public VKBotController(ApplicationDbContext dbContext, IHubContext<ChatHub> hubContext, IQueueService queue)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
            this.queue = queue;
        }

        [HttpPost]
        [Route("addplayer")]
        public async Task<IActionResult> CreatePlayer([FromBody]PlayerCreate createRequest)
        {
            if (dbContext.Players.FirstOrDefault(p => p.VKId == createRequest.VKId) != null)
            {
                var player = dbContext.Players.FirstOrDefault(p => p.VKId == createRequest.VKId);
                player.Status = PlayerStatus.InQueue;
                Score score = new Score()
                {
                    GameType = dbContext.GameTypes.FirstOrDefault(t => t.GameName == createRequest.GameName),
                    ScoreCount = default,
                    Time = DateTime.Now,
                    PlayerName = player
                };
                await dbContext.AddAsync(score);
                queue.PutInQueue(score.PlayerId);
            }
            else
            {
                Score score = new Score()
                {
                    GameType = dbContext.GameTypes.FirstOrDefault(t => t.GameName == createRequest.GameName),
                    ScoreCount = default,
                    Time = DateTime.Now,
                    PlayerName = new Player()
                    {
                        Username = createRequest.Username,
                        VKId = createRequest.VKId,
                        Status = PlayerStatus.InQueue
                    }
                };
                await dbContext.AddAsync(score);
                queue.PutInQueue(score.PlayerId);
            }
            await dbContext.SaveChangesAsync();
            await hubContext
                .Clients
                .All
                .SendAsync("New", createRequest.GameName, createRequest.VKId, createRequest.Username);
            return Ok();
        }

        //    [HttpGet]
        //    public IActionResult Get(string playerName)
        //    {
        //        var result = dbContext
        //            .Players
        //            .Where(p => p.Username == playerName)
        //            .Select(p => p.Scores)
        //            .ToList();

        //        return Json(result);
        //    }
        //}
    }
}