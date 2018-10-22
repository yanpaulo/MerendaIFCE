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
        private const string JsonContentType = "application/json";

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
            return await EnviaAsync<List<Inscricao>>($"Inscricoes?alteracao={ultimaAlteracao?.ToString("o")}", client.GetAsync);
        }

        public async Task<IList<Confirmacao>> GetConfirmacoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            var list = await EnviaAsync<List<ConfirmacaoDTO>>($"Confirmacoes?alteracao={ultimaAlteracao?.ToString("o")}", client.GetAsync);
            return Mapper.Map<List<Confirmacao>>(list);
        }

        public async Task<IList<ConfirmacaoDTO>> PostConfirmacoesAsync(IEnumerable<Confirmacao> confirmacaos)
        {
            var list = Mapper.Map<List<ConfirmacaoDTO>>(confirmacaos);
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var content = new StringContent(JsonConvert.SerializeObject(confirmacaos, settings), Encoding.UTF8, JsonContentType);
            var result = await client.PostAsync("Confirmacoes", content);
            return await HandleResponseAsync<List<ConfirmacaoDTO>>(result);
        }

        public async Task<T> EnviaAsync<T>(string url, Func<string, Task<HttpResponseMessage>> method)
        {
            var response = await method(url);
            return await HandleResponseAsync<T>(response);
        }

        public async Task<T> EnviaAsync<T>(object item, string url, Func<string, HttpContent, Task<HttpResponseMessage>> method)
        {
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, JsonContentType);
            var response = await method(url, content);
            return await HandleResponseAsync<T>(response);
        }

        private static async Task<T> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var ret = JsonConvert.DeserializeObject<T>(result);
                return ret;
            }

            throw new ApplicationException($"Erro de servidor ({response.StatusCode}): {result}");
        }
    }
}
