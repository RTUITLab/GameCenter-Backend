using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Requests;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/GameType")]
    public class GameTypeController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public GameTypeController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
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
                GameName = gameType
            };

            await dbContext.GameTypes.AddAsync(type);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        //Получение всех типов игр, хранящихся в таблице 
        [HttpGet]
        public IActionResult GetAllTypes()
        {
            var result = dbContext.GameTypes.Select(t => new
            {
                Name = t.GameName,
                GameId = t.GameTypeId
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
            return Ok();
        }

        //Переименование игр
        [HttpPut]
        [Route("rename/{gameType}")]
        public async Task<IActionResult> Rename(string gameType, [FromBody]GameTypeEdit editRequest)
        {
            if (!dbContext.GameTypes.Any(t => t.GameName == gameType))
            {
                NotFound();
            }

            var result = dbContext.GameTypes.FirstOrDefault(t => t.GameName == gameType);

            if (editRequest.GameName != null)
            {
                result.GameName = editRequest.GameName;
            }

            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}