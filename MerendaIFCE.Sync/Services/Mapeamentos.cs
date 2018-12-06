using AutoMapper;
using log4net;
using MerendaIFCE.Sync.DTO;
using MerendaIFCE.Sync.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerendaIFCE.Sync.Services
{
    public class Mapeamentos
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Mapeamentos));

        public static void Configura()
        {
            log.Debug("Configurando mapeamentos.");

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Confirmacao, ConfirmacaoDTO>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.IdRemoto));

                cfg.CreateMap<ConfirmacaoDTO, Confirmacao>()
                    .ForMember(dest => dest.IdRemoto, opt => opt.MapFrom(src => src.Id));

            });
        }
    }
}
