using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerendaIFCE.UserApp.Models
{
    public class Usuario
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        [Ignore]
        public Inscricao Inscricao { get; set; }
    }
}
