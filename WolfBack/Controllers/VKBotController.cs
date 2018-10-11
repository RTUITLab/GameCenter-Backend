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
            if (!dbContext.GameTypes.Any(t => t.GameName == createRequest.GameName))
            {
                return BadRequest("Wrong GameType");

            }
            if (dbContext.Players.FirstOrDefault(p => p.VKId == createRequest.VKId) != null)
            {
                var GameType = dbContext.GameTypes.FirstOrDefault(t => t.GameName == createRequest.GameName);
                var player = dbContext.Players.FirstOrDefault(p => p.VKId == createRequest.VKId);
                player.Status = PlayerStatus.InQueue;
                player.Username = createRequest.Username;

                Score score = new Score()
                {
                    GameType = dbContext.GameTypes.FirstOrDefault(t => t.GameName == createRequest.GameName),
                    ScoreCount = default,
                    Time = DateTime.Now,
                    PlayerName = player
                };

                await dbContext.AddAsync(score);
                await dbContext.SaveChangesAsync();
                queue.PutInQueue(score);
                await hubContext
                .Clients
                .All
                .SendAsync("Accept", new { createRequest.Username, createRequest.GameName });
                return Json(score.PlayerId);
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
                await dbContext.SaveChangesAsync();
                queue.PutInQueue(score);
                await hubContext
                .Clients
                .All
                .SendAsync("Accept", new { createRequest.Username, createRequest.GameName });
                return Json(score.PlayerId);
            }
        }

        [HttpGet]
        [Route("getqueue")]
        public IActionResult GetQueue()
        {
            return Json(queue.GetQueue(queue.GetCount())
                .Select(s => new
                {
                    s.GameType.GameName,
                    s.GameTypeId,
                    s.PlayerName.Username,
                    Status = s.PlayerName.Status.ToString(),
                    s.PlayerId
                }));
        }

        [HttpGet]
        [Route("getgames")]
        public IActionResult GetSelectedGames()
        {
            return Json(dbContext
                .GameTypes
                .Where(t => t.State == GameState.Selected)
                .Select(n => new { n.GameName, n.GameTypeId })
                );
        }

        [HttpGet]
        [Route("getmyscores/{playerId}")]
        public IActionResult GetMyScores(Guid playerId)
        {
            var res = dbContext.Scores.Where(s => s.PlayerId == playerId).OrderByDescending(c => c.ScoreCount).Select(u => new { u.GameType.GameName, u.ScoreCount }).Take(5);
            return Json(res);
        }
    }
}