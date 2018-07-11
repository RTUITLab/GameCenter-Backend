using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Models;
using Microsoft.EntityFrameworkCore;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/Scores")]
    public class ScoresController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public ScoresController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        //Добавление нового игрока в таблицу
        [HttpPost]
        [Route("{username}/{gameType}/{score}")]
        public async Task<IActionResult> PostScore(string username, int score, string gameType)
        {
            if (await dbContext.Scores
                .Where(s => s.GameType.GameName == gameType)
                .Where(s => s.PlayerName.Username == username)
                .AnyAsync())
            {
                return BadRequest();
            }

            Score playerScore = new Score
            {
                GameType = dbContext
                .GameTypes
                .FirstOrDefault(t => t.GameName == gameType),
                ScoreCount = score,
                PlayerName = new Player
                {
                    Username = username,
                }
            };

            await dbContext.Scores.AddAsync(playerScore);
            await dbContext.SaveChangesAsync();

            return Ok();
        }

        //Получение первых пяти лучших результатов в зависимости от типа игры
        [HttpGet]
        [Route("{gameType}")]
        public async Task<IActionResult> GetScore(string gameType)
        {
            if (await dbContext.GameTypes.FindAsync(gameType) == null)
            {
                return BadRequest();
            }

            var result = dbContext
                .Scores
                .Where(type => type.GameType.GameName == gameType)
                .OrderByDescending(s => s.ScoreCount)
                .Select(s => new
                {
                    score = s.ScoreCount,
                    name = s.PlayerName.Username
                })
                .Take(5);
            return Json(result);
        }
    }
}