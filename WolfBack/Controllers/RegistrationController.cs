using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.ModelViews;
using Models.Responces;
using WolfBack.Services.Interfaces;
using WolfBack.SignalR;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/registration")]
    public class RegistrationController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHubContext<ChatHub> hubContext;
        private readonly IQueueService queue;

        public RegistrationController(
            ApplicationDbContext dbContext,
            IHubContext<ChatHub> hubContext,
            IQueueService queue)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
            this.queue = queue;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody]PlayerCreateRequest request)
        {
            var game = dbContext
                .GameTypes
                .FirstOrDefault(g => g.GameTypeId == request.GameId && g.State == GameState.Selected);

            if (game == null)
            {
                return NotFound("Игра не найдена");
            }

            Player player = new Player()
            {
                Username = request.Username,
                Score = new Score()
                {
                    GameType = game,
                    ScoreCount = default,
                    Time = DateTime.Now
                },
                Status = PlayerStatus.InQueue
            };

            await dbContext.AddAsync(player);
            await dbContext.SaveChangesAsync();
            queue.PutInQueue(player);
            await hubContext.Clients.All.SendAsync("Add", request.Username, game.GameName);

            return Ok("Вы добавлены в очередь");
        }

        [HttpGet]
        [Route("queue")]
        public IActionResult GetQueue()
        {
            return Json(queue.GetQueue(queue.GetCount())
                .Select(s => new QueueResponce
                {
                    GameId = s.Score.GameTypeId,
                    PlayerId = s.PlayerId,
                    UserName = s.Username,
                    GameName = s.Score.GameType.GameName,
                    Status = s.Status.ToString()
                }));
        }
    }
}