using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using WolfBack.Services.Interfaces;
using WolfBack.SignalR;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/PlayerManager")]
    public class PlayerManagerController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IQueueService queue;
        private readonly IHubContext<ChatHub> hubContext;

        public PlayerManagerController(ApplicationDbContext dbContext,
            IQueueService queue,
            IHubContext<ChatHub> hubContext)
        {
            this.dbContext = dbContext;
            this.queue = queue;
            this.hubContext = hubContext;
        }

        [HttpPut]
        [Route("accept/{userId}")]
        public async Task<IActionResult> AcceptPlayer(Guid userId)
        {
            var res = dbContext.Players.FirstOrDefault(t => t.PlayerId == userId);
            res.Status = PlayerStatus.InGame;
            queue.FindInQueue(userId).PlayerName.Status = PlayerStatus.InGame;
            await dbContext.SaveChangesAsync();

            await hubContext.Clients.All.SendAsync("Accept", userId, res.Status.ToString());

            return Ok();
        }

        [HttpPut]
        [Route("refuse/{userId}/{gameId}/{score}")]
        public async Task<IActionResult> RefusePlayer(Guid userId, Guid gameId, int score)
        {
            var res = dbContext.Players.FirstOrDefault(t => t.PlayerId == userId);
            var game = dbContext.Scores.FirstOrDefault(u => u.GameType.GameTypeId == gameId && u.PlayerId == userId);
            res.Status = PlayerStatus.Free;
            game.ScoreCount = score;
            queue.DeletePlayer(dbContext.Scores.FirstOrDefault(s => s.PlayerId == userId));
            await dbContext.SaveChangesAsync();

            await hubContext.Clients.All.SendAsync("Accept", userId);

            return Ok();
        }

        [HttpPut]
        [Route("deleteall/{gameId}")]
        public async Task<IActionResult> DeleteAll(Guid gameId)
        {
            var res = await dbContext.GameTypes.FindAsync(gameId);
            queue.DeletePlayers(res);
            await hubContext.Clients.All.SendAsync("Accept", gameId);
            return Ok();
        }
    }
}