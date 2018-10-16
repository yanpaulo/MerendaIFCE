﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MerendaIFCE.UserApp.Models
{
    public class Inscricao
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        public string Matricula { get; set; }

        public List<InscricaoDia> Dias { get; set; }

        public DateTimeOffset UltimaModificacao { get; set; }

        public ICollection<Confirmacao> Confirmacoes { get; set; }
    }
}
