using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Models.Requests;
using WolfBack.Services.Interfaces;
using WolfBack.Settings;

namespace WolfBack.Controllers
{
    [Produces("application/json")]
    [Route("api/Authorize")]
    public class AuthorizeController : Controller
    {
        private readonly IKeyService keyService;
        private readonly IConfiguration configuration;
        private readonly DefaultAdmin options;

        public AuthorizeController(
            IKeyService keyService,
            IConfiguration configuration,
            IOptions<DefaultAdmin> options)
        {
            this.keyService = keyService;
            this.configuration = configuration;
            this.options = options.Value;
        }

        [HttpPost]
        public IActionResult Authorize([FromBody]AuthRequest model)
        {
            if (options.LoginPairs.FirstOrDefault(u => u.Password == model.Password && u.Login == model.Login) == null)
            {
                return BadRequest();
            }
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: configuration.GetSection("JwtOptions")["Issuer"],
                audience: configuration.GetSection("JwtOptions")["Audience"],
                notBefore: now,
                claims: GetIdentity(model).Claims,
                expires: now.Add(TimeSpan.FromMinutes(2)),
                signingCredentials: keyService.GetSecretKey()
                );
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return Json(new
            {
                model.Login,
                Token = encodedJwt
            });
        }

        private ClaimsIdentity GetIdentity(AuthRequest admin)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType, admin.Password),
                };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(
                claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

            return claimsIdentity;
        }
    }
}