using MerendaIFCE.Sync.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MerendaIFCE.Sync.Services
{
    class SyncWebService
    {
        private HttpClient client;

        public SyncWebService()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("http://merenf.yan-soft.com/api")
            };
        }

        public async Task<IList<Inscricao>> GetInscricoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            var response = await client.GetAsync($"Inscricoes?alteracao={ultimaAlteracao}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<Inscricao>>(content);
                return result.Select(r => Inscricao.ConverteRemota(r)).ToList();
            }

            throw new ApplicationException($"Erro no servidor ({response.StatusCode}): {content}");
        }
    }
}
