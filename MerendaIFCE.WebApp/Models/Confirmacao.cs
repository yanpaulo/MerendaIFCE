using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Models
{
    public class Confirmacao
    {
        public int Id { get; set; }

        public string Mensagem { get; set; }

        public DateTimeOffset Dia { get; set; }

        public DateTimeOffset UltimaModificacao { get; set; }

        public StatusConfirmacao StatusConfirmacao { get; set; }

        public int InscricaoId { get; set; }
    }

    public enum StatusConfirmacao
    {
        NaoConfirmado,
        Confirmado,
        Erro
    }
}
