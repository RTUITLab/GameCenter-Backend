using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.Requests;
using WolfBack.SignalR;
using Models.Responces;
using Microsoft.AspNetCore.Http;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/GameType")]
    public class GameTypeController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHubContext<ChatHub> hubContext;

        public GameTypeController(
            ApplicationDbContext dbContext,
            IHubContext<ChatHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        //Добавление нового типа игры в таблицу
        [HttpPost]
        public async Task<IActionResult> PostGameType([FromBody] GameTypeCreateRequest request)
        {
            if (dbContext.GameTypes.Any(g => g.GameName == request.GameType))
            {
                return BadRequest();
            }

            GameType game = new GameType
            {
                GameName = request.GameType,
                State = GameState.NotSelected,
                ImageURL = request.ImageURL
            };

            await dbContext.GameTypes.AddAsync(game);
            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Add", game.GameName);

            return Ok();
        }

        //Получение всех типов игр, хранящихся в таблице 
        [HttpGet]
        [Route("getall")]
        public IActionResult GetAllTypes()
        {
            return Json(dbContext
                .GameTypes
                .Select(t => new GameTypeResponse()
                {
                    GameId = t.GameTypeId,
                    GameName = t.GameName
                }));
        }

        //Удаление игры
        [HttpDelete]
        [Route("delete/{gameId}")]
        public async Task<IActionResult> DeleteGame(Guid gameId)
        {
            if (!dbContext.GameTypes.Any(t => t.GameTypeId == gameId))
            {
                return NotFound();
            }

            var game = await dbContext.GameTypes.FindAsync(gameId);
            dbContext.GameTypes.Remove(game);
            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Delete", game.GameName);
            return Ok();
        }

        //Переименование игр
        [HttpPut]
        [Route("rename")]
        public async Task<IActionResult> Rename([FromBody]GameTypeEditRequest request)
        {
            if (!dbContext.GameTypes.Any(t => t.GameTypeId == request.GameId))
            {
                return NotFound("Игра не найдена");
            }

            var game = await dbContext.GameTypes.FindAsync(request.GameId);

            if (request.GameName != null)
            {
                game.GameName = request.GameName;
            }

            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Rename", request.GameName);
            return Ok();
        }

        //Выбор показываемых на стендах игр
        [HttpPut]
        [Route("pickgame")]
        public async Task<IActionResult> PickGame([FromBody]IdRequest request)
        {
            var game = await dbContext.GameTypes.FindAsync(request.Id);

            if (game.State == GameState.Selected)
            {
                return BadRequest("Игра уже выбрана");
            }

            game.State = GameState.Selected;

            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Pick", request.Id);
            return Ok();
        }

        //Отмена выбора показывавемых на стендах игр
        [HttpPut]
        [Route("unpickgame")]
        public async Task<IActionResult> UnpickGames([FromBody]IdRequest request)
        {
            var game = await dbContext.GameTypes.FindAsync(request.Id);
            game.State = GameState.NotSelected;

            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("Pick", request);
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
                .Select(t => new GameTypeResponse()
                {
                    GameId = t.GameTypeId,
                    GameName = t.GameName,
                    ImageURL = t.ImageURL
                }));
        }
    }
}