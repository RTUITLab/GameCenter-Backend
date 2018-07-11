using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;

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

        // Получение всех типов игр, хранящихся в таблице 
        [HttpGet]
        public IActionResult GetAllTypes()
        {
            var result = dbContext.GameTypes.Select(t => new
            {
                Name = t.GameName,
                GameTypeId = t.Id
            });

            return Json(result);
        }
    }
}