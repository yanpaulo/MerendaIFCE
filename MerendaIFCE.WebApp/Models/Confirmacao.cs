using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Models
{
    public class Confirmacao
    {
        public int Id { get; set; }

        public DateTimeOffset Dia { get; set; }

        public InscricaoDia InscricaoDia { get; set; }

        public StatusConfirmacao StatusConfirmacao { get; set; }
    }

    public enum StatusConfirmacao
    {
        NaoConfirmado,
        Confirmado,
        Erro
    }

    public enum StatusSincronia
    {
        Modificado,
        Sincronizado,
        Erro
    }
}
