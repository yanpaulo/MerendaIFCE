using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Models
{
    public class InscricaoDia
    {
        public int Id { get; set; }

        public DayOfWeek Dia { get; set; }

        public int InscricaoId { get; set; }
    }
}
