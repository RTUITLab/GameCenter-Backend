using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using WolfBack.SignalR;

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

        //Получение первых пяти лучших результатов в зависимости от типа игры

        [HttpGet]
        [Route("{gameType}")]
        public async Task<IActionResult> GetScore(string gameType)
        {
            if (!await dbContext.GameTypes.AnyAsync(t => t.GameName == gameType))
            {
                return BadRequest();
            }

            var result = dbContext
                .Scores
                .Where(type => type.GameType.GameName == gameType)
                .OrderByDescending(s => s.ScoreCount)
                .Select(s => new
                {
                    Name = s.PlayerName.Username,
                    Score = s.ScoreCount
                })
                .Take(3);
            return Json(result);
        }

        [HttpGet]
        [Route("getmyscores/{VkId}")]
        public IActionResult GetMyScores(string VkId)
        {
            return Json(dbContext
                .Players
                .FirstOrDefault(u => u.VKId == VkId).Scores);
        }
    }
}