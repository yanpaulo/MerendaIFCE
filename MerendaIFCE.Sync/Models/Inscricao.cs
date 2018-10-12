using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.Models
{
    public class Inscricao
    {
        public int Id { get; set; }

        public int IdRemoto { get; set; }

        [Required]
        public string Matricula { get; set; }

        public List<InscricaoDia> Dias { get; set; }

        public DateTimeOffset UltimaModificacao { get; set; }

        public static Inscricao ConverteRemota(Inscricao inscricao)
        {
            InscricaoDia Converte(InscricaoDia inscricaoDia) =>
               new InscricaoDia
               {
                   Dia = inscricaoDia.Dia
               };

            return new Inscricao
            {
                IdRemoto = inscricao.Id,
                Matricula = inscricao.Matricula,
                UltimaModificacao = inscricao.UltimaModificacao,
                Dias = inscricao.Dias.Select(Converte).ToList()
            };
           
        }
    }
}
