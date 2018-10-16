using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MerendaIFCE.WebApp.Models
{
    public class Inscricao
    {
        public int Id { get; set; }

        [Required]
        public string Matricula { get; set; }

        public List<InscricaoDia> Dias { get; set; }

        public DateTimeOffset UltimaModificacao { get; set; }

        [JsonIgnore]
        public ICollection<Confirmacao> Confirmacoes { get; set; }

    }
}
