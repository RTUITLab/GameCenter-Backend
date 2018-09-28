using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WolfBack.Services.Interfaces
{
    public interface IKeyService
    {
        SigningCredentials GetSecretKey();
    }
}
