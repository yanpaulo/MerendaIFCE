using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SQLite;

namespace MerendaIFCE.UserApp.Models
{
    public class InscricaoDia
    {
        [PrimaryKey]
        public int Id { get; set; }

        public DayOfWeek Dia { get; set; }

        public int InscricaoId { get; set; }
    }
}
