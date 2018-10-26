using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using WolfBack.Services.Interfaces;
using WolfBack.SignalR;
using WolfBackAPI.Requests;
using WolfBackAPI.Responces;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("WolfBackAPI")]
    public class WolfBackAPIController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IQueueService queue;
        private readonly IHubContext<ChatHub> hubContext;

        public WolfBackAPIController(
            ApplicationDbContext dbContext,
            IQueueService queue,
            IHubContext<ChatHub> hubContext)
        {
            this.dbContext = dbContext;
            this.queue = queue;
            this.hubContext = hubContext;
        }

        [HttpPost]
        [Route("score")]
        public async Task<IActionResult> PostScore([FromBody] ScorePostRequest request)
        {
            var player = queue.GetFirstInGameQueue(request.GameId);

            player.Score.ScoreCount = request.Score;
            queue.DeletePlayer(player.PlayerId);

            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("addscore", player.Username, player.Score);
            return Ok();
        }

        [HttpGet]
        [Route("game/{gameName}")]
        public IActionResult GetGameId(string gameName)
        {
            if (!dbContext.GameTypes.Any(n => n.GameName == gameName))
            {
                return BadRequest("Игра не найдена");

            }
            return Json(dbContext
                .GameTypes
                .Where(g => g.GameName == gameName)
                .Select(s => new GetGameIdResponce()
                {
                    GameId = s.GameTypeId
                }));
        }
    }
}