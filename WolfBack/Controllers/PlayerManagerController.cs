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
        [Route("accept/{username}")]
        public async Task<IActionResult> AcceptPlayer(string username)
        {
            var res = dbContext.Players.FirstOrDefault(t => t.Username == username);
            res.Status = PlayerStatus.InGame;
            queue.FindInQueue(username).PlayerName.Status = PlayerStatus.InGame;
            await dbContext.SaveChangesAsync();

            await hubContext.Clients.All.SendAsync("Accept", username, res.Status.ToString());

            return Ok();
        }

        [HttpPut]
        [Route("refuse/{username}/{score}")]
        public async Task<IActionResult> RefusePlayer(string username, int score)
        {
            var res = dbContext.Players.FirstOrDefault(t => t.Username == username);
            res.Status = PlayerStatus.Free;
            queue.DeletePlayer(dbContext.Scores.FirstOrDefault(s => s.PlayerName.Username == username));
            await dbContext.SaveChangesAsync();

            await hubContext.Clients.All.SendAsync("Accept", username);

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