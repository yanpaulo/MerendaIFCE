using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.Models
{
    public class InscricaoDia
    {
        public int Id { get; set; }

        public DayOfWeek Dia { get; set; }

        public Inscricao Inscricao { get; set; }

        public int InscricaoId { get; set; }

        public ICollection<Confirmacao> Confirmacoes { get; set; }
    }
}
