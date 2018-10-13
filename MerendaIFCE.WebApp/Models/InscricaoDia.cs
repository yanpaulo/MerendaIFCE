using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MerendaIFCE.WebApp.Models
{
    public class InscricaoDia
    {
        public int Id { get; set; }

        public DayOfWeek Dia { get; set; }

        public int InscricaoId { get; set; }

        [JsonIgnore]
        public ICollection<Confirmacao> Confirmacoes { get; set; }
    }
}
