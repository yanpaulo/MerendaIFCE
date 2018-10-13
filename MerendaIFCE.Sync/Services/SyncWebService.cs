using MerendaIFCE.Sync.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using MerendaIFCE.Sync.DTOs;

namespace MerendaIFCE.Sync.Services
{
    class SyncWebService
    {
        private HttpClient client;

        public SyncWebService()
        {
            client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:7354/api/")
            };
        }

        public async Task<IList<Inscricao>> GetInscricoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            var response = await client.GetAsync($"Inscricoes?alteracao={ultimaAlteracao?.ToString("o")}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var result = JsonConvert.DeserializeObject<List<Inscricao>>(content);
                return result;
            }

            throw new ApplicationException($"Erro ao obter inscrições do servidor ({response.StatusCode}): {content}");
        }

        public async Task PostConfirmacoesAsync(IEnumerable<Confirmacao> confirmacaos)
        {
            var list = Mapper.Map<List<ConfirmacaoDTO>>(confirmacaos);
            var content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("Confirmacoes", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Erro ao enviar confirmações para o servidor ({response.StatusCode}).");
            }
        }
    }
}
