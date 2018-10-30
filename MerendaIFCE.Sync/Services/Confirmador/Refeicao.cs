using System;
using System.Collections.Generic;
using System.Text;

namespace MerendaIFCE.Sync.Services.Confirmador
{
    public class Refeicao
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public int Quantidade { get; set; }

        public DateTimeOffset Data { get; set; }
    }

}
