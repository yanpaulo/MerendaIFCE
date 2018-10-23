using AutoMapper;
using MerendaIFCE.Sync.DTOs;
using MerendaIFCE.Sync.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerendaIFCE.Sync.Configurations
{
    public class Mapeamentos
    {
        public static void Configura()
        {
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
