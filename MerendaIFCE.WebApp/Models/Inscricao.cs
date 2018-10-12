using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Models
{
    public class Inscricao
    {
        public int Id { get; set; }

        public ICollection<InscricaoDia> Dias { get; set; }

        public DateTimeOffset UltimaModificacao { get; set; }
    }
}
