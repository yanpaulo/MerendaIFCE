using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.Models
{
    public class InscricaoDia
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public DayOfWeek Dia { get; set; }

        public Inscricao Inscricao { get; set; }

        public int InscricaoId { get; set; }
    }
}
