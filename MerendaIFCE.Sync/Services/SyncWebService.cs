using MerendaIFCE.Sync.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using MerendaIFCE.Sync.DTO;

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
            using (var db = new LocalDbContext())
            {
                var usuario = db.Usuario.SingleOrDefault();
                if (usuario != null)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);
                }
            }
        }

        public async Task<Usuario> LogInAsync()
        {
            using (var db = new LocalDbContext())
            {
                Login login = new Login();
                App.Current.Settings.GetSection("SyncUser").Bind(login);
                var usuario = await EnviaAsync<Usuario>(login, "Conta/Login", client.PostAsync);
                db.Usuario.RemoveRange(db.Usuario.ToList());
                db.Add(usuario);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

                db.SaveChanges();
                return usuario;
            }

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
            var content = new StringContent(JsonConvert.SerializeObject(list, settings), Encoding.UTF8, JsonContentType);
            var result = await client.PostAsync("Confirmacoes", content);
            return await HandleResponseAsync<List<ConfirmacaoDTO>>(result);
        }

        public async Task<T> EnviaAsync<T>(string url, Func<string, Task<HttpResponseMessage>> method, int retry = 0)
        {
            var response = await method(url);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && retry == 0)
            {
                await LogInAsync();
                return await EnviaAsync<T>(url, method, ++retry);
            }
            else
            {
                return await HandleResponseAsync<T>(response);
            }
        }

        public async Task<T> EnviaAsync<T>(object item, string url, Func<string, HttpContent, Task<HttpResponseMessage>> method, int retry = 0)
        {
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, JsonContentType);
            var response = await method(url, content);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && retry == 0)
            {
                await LogInAsync();
                return await EnviaAsync<T>(item, url, method, ++retry);
            }
            else
            {
                return await HandleResponseAsync<T>(response);
            }
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
