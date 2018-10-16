using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace MerendaIFCE.UserApp.Models
{
    public class Confirmacao
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

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
