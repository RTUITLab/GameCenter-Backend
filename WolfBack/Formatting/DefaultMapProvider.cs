using AutoMapper;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WolfBackAPI.Responces;

namespace WolfBack.Formatting
{
    public class DefaultMapProvider : Profile
    {
        public DefaultMapProvider()
        {
            CreateMap<GameType, GetGameIdResponce>()
                .ForMember(gir => gir.GameId, map => map.MapFrom(gt => gt.GameTypeId));
        }
    }
}
