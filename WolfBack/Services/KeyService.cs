using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfBack.Services.Interfaces;

namespace WolfBack.Services
{
    public class KeyService : IKeyService
    {
        private readonly IConfiguration configuration;
        private SigningCredentials SecretKey { get; set; }

        public KeyService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public SigningCredentials GetSecretKey()
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JwtOptions")["Key"]));
            SecretKey = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            return SecretKey;
        }
    }
}
