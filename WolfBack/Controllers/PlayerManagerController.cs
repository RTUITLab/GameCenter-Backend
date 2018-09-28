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

        public PlayerManagerController(ApplicationDbContext dbContext, IQueueService queue, IHubContext<ChatHub> hubContext)
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
            await dbContext.SaveChangesAsync();


            return Ok();
        }
    }
}