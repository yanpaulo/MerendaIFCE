using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.Models
{
    public class Confirmacao
    {
        public int Id { get; set; }

        public int? IdRemoto { get; set; }

        public string Mensagem { get; set; }

        public DateTimeOffset? UltimaModificacao { get; set; }

        public DateTimeOffset Dia { get; set; }

        public StatusConfirmacao StatusConfirmacao { get; set; }

        public StatusSincronia StatusSincronia { get; set; }

        [JsonIgnore]
        public Inscricao Inscricao { get; set; }

        public int InscricaoId { get; set; }
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
