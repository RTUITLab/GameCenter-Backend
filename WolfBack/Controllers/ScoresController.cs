using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Models;
using Models.Requests;
using Models.Responces.Scores;
using WolfBack.SignalR;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/Scores")]
    public class ScoresController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IHubContext<ChatHub> hubContext;

        public ScoresController(ApplicationDbContext dbContext, IHubContext<ChatHub> hubContext)
        {
            this.dbContext = dbContext;
            this.hubContext = hubContext;
        }

        [HttpGet]
        [Route("{gameTypeId}")]
        public async Task<IActionResult> GetScore(Guid gameTypeId)
        {
            if (await dbContext.GameTypes.FindAsync(gameTypeId) == null)
            {
                return BadRequest("Игра не найдена");
            }

            return Json(dbContext
                .Scores
                .Where(id => id.GameTypeId == gameTypeId)
                .OrderByDescending(s => s.ScoreCount)
                .Select(s => new ScoreResponce()
                {
                    ScoreId = s.ScoreId,
                    UserName = s.Player.Username,
                    GameName = s.GameType.GameName,
                    Score = s.ScoreCount,
                    Date = DateTime.Now
                }));
        }

        [HttpGet]
        [Route("top")]
        public IActionResult GetTopScores()
        {
            return Json(dbContext
                .GameTypes
                .Where(s => s.State == GameState.Selected)
                .Select(s => s
                    .Scores
                    .OrderByDescending(b => b.ScoreCount)
                    .Select(n => new TopScoreResponce()
                    {
                        UserName = n.Player.Username,
                        GameName = n.GameType.GameName,
                        Score = n.ScoreCount,
                    })
                .Take(3))
                .ToList());
        }

        [HttpGet]
        [Route("last")]
        public IActionResult GetLastScores()
        {
            return Json(dbContext
                .GameTypes
                .Where(s => s.State == GameState.Selected)
                .Select(s => s
                    .Scores.OrderByDescending(b => b.ScoreCount)
                    .Select(n => new TopScoreResponce()
                    {
                        UserName = n.Player.Username,
                        GameName = n.GameType.GameName,
                        Score = n.ScoreCount
                    })
                    .TakeLast(5))
                .ToList());
        }

        [HttpDelete]
        [Route("delete_all/{gameId}")]
        public async Task<IActionResult> DeleteAll(Guid gameId)
        {
            var scores = dbContext.Scores.Where(id => id.GameTypeId == gameId);
            dbContext.Scores.RemoveRange(scores);
            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("DeleteRecord", gameId);
            return Ok();
        }

        [HttpDelete]
        [Route("delete/{playerId}")]
        public async Task<IActionResult> Delete(Guid playerId)
        {
            var score = dbContext.Scores.FirstOrDefault(s => s.ScoreId == playerId);
            dbContext.Scores.Remove(score);
            await dbContext.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("DeleteRecord", score.PlayerId);
            return Ok();
        }
    }
}