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
        [Route("{gameId}")]
        public async Task<IActionResult> GetScore(Guid gameId)
        {
            if (!await dbContext.GameTypes.AnyAsync(t => t.GameTypeId == gameId))
            {
                return BadRequest();
            }

            var result = dbContext
                .Scores
                .Where(type => type.GameTypeId == gameId)
                .OrderByDescending(s => s.ScoreCount)
                .Select(s => new
                {
                    Name = s.PlayerName.Username,
                    Score = s.ScoreCount,
                    Date = s.Time,
                    s.ScoreId
                });
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetTopScores()
        {
            var result = dbContext
                .GameTypes
                .Where(s => s.State == GameState.Selected)
                .Select(s => s.Scores.OrderByDescending(b => b.ScoreCount).Select(n => new { n.GameType.GameName, n.PlayerName.Username, n.ScoreCount }).Take(3))
                .ToList();

            return Json(result);
        }

        [HttpGet]
        [Route("last")]
        public IActionResult GetLastScores()
        {
            var result = dbContext
                .GameTypes
                .Where(s => s.State == GameState.Selected)
                .Select(s => s.Scores.OrderByDescending(b => b.ScoreCount).Select(n => new { n.GameType.GameName, n.PlayerName.Username, n.ScoreCount }).TakeLast(5))
                .ToList();

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

        [HttpDelete]
        [Route("delete_all/{gameTypeId}")]
        public async Task<IActionResult> DeleteAll(Guid gameTypeId)
        {
            var scores = dbContext.Scores.Where(id => id.GameTypeId == gameTypeId);
            dbContext.Scores.RemoveRange(scores);
            await dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete]
        [Route("delete/{scoreId}")]
        public async Task<IActionResult> Delete(Guid scoreId)
        {
            var score = dbContext.Scores.FirstOrDefault(s => s.ScoreId == scoreId);
            dbContext.Scores.Remove(score);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}