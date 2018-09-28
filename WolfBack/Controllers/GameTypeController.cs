using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.Requests;
using WolfBack.SignalR;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/GameType")]
    public class GameTypeController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHubContext<ChatHub> hubContext;

        public GameTypeController(ApplicationDbContext dbContext, IHubContext<ChatHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        //Добавление нового типа игры в таблицу
        [HttpPost]
        [Route("{gameType}")]
        public async Task<IActionResult> PostGameType(string gameType)
        {
            if (dbContext.GameTypes.Any(g => g.GameName == gameType))
            {
                return BadRequest();
            }

            GameType type = new GameType
            {
                GameName = gameType,
                State = GameState.NotSelected
            };

            await dbContext.GameTypes.AddAsync(type);
            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Add", gameType);

            return Ok();
        }

        //Получение всех типов игр, хранящихся в таблице 
        [HttpGet]
        public IActionResult GetAllTypes()
        {
            var result = dbContext.GameTypes.Select(t => new
            {
                Name = t.GameName,
                GameId = t.GameTypeId,
                State = t.State.ToString()
            });

            return Json(result);
        }

        //Удаление игры
        [HttpDelete]
        [Route("delete/{gameType}")]
        public async Task<IActionResult> DeleteGame(string gameType)
        {
            if (!dbContext.GameTypes.Any(t => t.GameName == gameType))
            {
                return NotFound();
            }

            var del = dbContext.GameTypes.FirstOrDefault(t => t.GameName == gameType);
            dbContext.GameTypes.Remove(del);
            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Delete", gameType);
            return Ok();
        }

        //Переименование игр
        [HttpPut]
        [Route("rename/{gameType}")]
        public async Task<IActionResult> Rename(string gameType, [FromBody]GameTypeEdit editRequest)
        {
            if (!dbContext.GameTypes.Any(t => t.GameName == gameType))
            {
                return NotFound();
            }

            var result = dbContext.GameTypes.FirstOrDefault(t => t.GameName == gameType);

            if (editRequest.GameName != null)
            {
                result.GameName = editRequest.GameName;
            }

            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Rename", editRequest.GameName);
            return Ok();
        }

        //Выбор показываемых на стендах игр
        [HttpPut]
        [Route("pickgames")]
        public async Task<IActionResult> PickGame([FromBody]List<Guid> pickRequest)
        {
            foreach (var gameID in pickRequest)
            {
                var result = await dbContext.GameTypes.FindAsync(gameID);
                if (result.State == GameState.Selected)
                {
                    return BadRequest();
                }
                result.State = GameState.Selected;
                await dbContext.SaveChangesAsync();
            }
            await hubContext.Clients.All.SendAsync("Pick", pickRequest);
            return Ok();
        }

        //Отмена выбора показывавемых на стендах игр
        [HttpPut]
        [Route("unpickgames")]
        public async Task<IActionResult> UnpickGames([FromBody]List<Guid> unpickRequest)
        {
            foreach (var gameID in unpickRequest)
            {
                var result = await dbContext.GameTypes.FindAsync(gameID);
                result.State = GameState.NotSelected;
                await dbContext.SaveChangesAsync();
            }
            await hubContext.Clients.All.SendAsync("Pick", unpickRequest);
            return Ok();
        }

        //Получение выбранных игр
        [HttpGet]
        [Route("selected")]
        public IActionResult GetSelectedGames()
        {
            return Json(dbContext
                .GameTypes
                .Where(t => t.State == GameState.Selected)
                .Select(t => new
                {
                    Name = t.GameName,
                    GameId = t.GameTypeId,
#if DEBUG
                    State = t.State.ToString()
#endif
                }));
        }
    }
}