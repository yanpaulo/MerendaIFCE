using System;
using System.Linq;
using System.Collections.Generic;
using SQLite;

namespace MerendaIFCE.UserApp.Models
{
    public class Inscricao
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Matricula { get; set; }

        [Ignore]
        public List<InscricaoDia> Dias { get; set; }

        public DateTimeOffset UltimaModificacao { get; set; }

        [Ignore]
        public ICollection<Confirmacao> Confirmacoes { get; set; }

        

    }
}
