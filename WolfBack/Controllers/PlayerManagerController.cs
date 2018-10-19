using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.Requests;
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
        [Route("accept")]
        public async Task<IActionResult> AcceptPlayer([FromBody] IdRequest request)
        {
            if (await dbContext.Players.FindAsync(request.Id) == null)
            {
                return BadRequest("игрок не найден");
            }

            var player = dbContext.Players.FirstOrDefault(t => t.PlayerId == request.Id);
            player.Status = PlayerStatus.InGame;
            queue.FindInQueue(request.Id).Status = PlayerStatus.InGame;

            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Accept", request.Id, player.Status.ToString());
            return Ok();
        }

        [HttpPut]
        [Route("refuse/{userId}/{gameId}/{score}")]
        public async Task<IActionResult> RefusePlayer(Guid playerId, Guid gameId, int score)
        {
            var player = dbContext.Players.FirstOrDefault(t => t.PlayerId == playerId);
            var game = dbContext.Scores.FirstOrDefault(u => u.GameType.GameTypeId == gameId && u.PlayerId == playerId);

            player.Status = PlayerStatus.Free;
            game.ScoreCount = score;
            queue.DeletePlayer(playerId);

            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Accept", playerId);
            return Ok();
        }

        [HttpDelete]
        [Route("deleteall/{gameId}")]
        public async Task<IActionResult> DeleteAll(Guid gameId)
        {
            queue.DeletePlayers(gameId);
            await hubContext.Clients.All.SendAsync("Accept", gameId);
            return Ok();
        }
    }
}