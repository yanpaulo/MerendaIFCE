﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace MerendaIFCE.UserApp.Models
{
    public class Confirmacao
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Mensagem { get; set; }

        public bool Cancela { get; set; }

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
