using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Models
{
    public class Confirmacao
    {
        public int Id { get; set; }

        public DateTimeOffset Dia { get; set; }

        public InscricaoDia InscricaoDia { get; set; }

        //Ambos para se utilizar no app de SYNC!!
        public StatusConfirmacao StatusConfirmacao { get; set; }

        public StatusEnvio StatusEnvio { get; set; }
    }

    public enum StatusConfirmacao
    {
        NaoConfirmado,
        Confirmado,
        Erro
    }

    public enum StatusEnvio
    {
        Criado,
        Sincronizado,
        Erro
    }
}
