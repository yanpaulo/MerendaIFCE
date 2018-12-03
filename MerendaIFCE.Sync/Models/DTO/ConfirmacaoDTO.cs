using MerendaIFCE.Sync.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.DTO
{
    public class ConfirmacaoDTO
    {
        public int Id { get; set; }

        public DateTimeOffset Dia { get; set; }

        public string Mensagem { get; set; }

        public int InscricaoId { get; set; }

        public StatusConfirmacao StatusConfirmacao { get; set; }

        public DateTimeOffset? UltimaModificacao { get; set; }
    }
}
