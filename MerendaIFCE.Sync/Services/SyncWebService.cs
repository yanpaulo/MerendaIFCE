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
                var content = new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, JsonContentType);
                var response = await RequestAsync(async () => await client.PostAsync("Conta/Login", content), logIn: false);
                var usuario = JsonConvert.DeserializeObject<Usuario>(response);
                db.Usuario.RemoveRange(db.Usuario.ToList());
                db.Add(usuario);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

                db.SaveChanges();
                return usuario;
            }

        }

        public async Task<IList<Inscricao>> GetInscricoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            return await RequestAsync<List<Inscricao>>(client.GetAsync, $"Inscricoes?alteracao={ultimaAlteracao?.ToString("o")}");
        }

        public async Task<IList<Confirmacao>> GetConfirmacoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            var list = await RequestAsync<List<ConfirmacaoDTO>>(client.GetAsync, $"Confirmacoes?alteracao={ultimaAlteracao?.ToString("o")}");
            return Mapper.Map<List<Confirmacao>>(list);
        }

        public async Task<IList<ConfirmacaoDTO>> PostConfirmacoesAsync(IEnumerable<Confirmacao> confirmacoes)
        {
            var list = Mapper.Map<List<ConfirmacaoDTO>>(confirmacoes);
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            var content = new StringContent(JsonConvert.SerializeObject(list, settings), Encoding.UTF8, JsonContentType);
            return await RequestAsync<List<ConfirmacaoDTO>>(client.PostAsync, "Confirmacoes", content);
        }
        
        private async Task<T> RequestAsync<T>(Func<string, Task<HttpResponseMessage>> method, string url)
        {
            var response = await RequestAsync(method, url);
            var obj = JsonConvert.DeserializeObject<T>(response);
            return obj;
        }

        private async Task<T> RequestAsync<T>(Func<string, HttpContent, Task<HttpResponseMessage>> method, string url, object content)
        {
            var str = JsonConvert.SerializeObject(content);
            return await RequestAsync<T>(method, url, new StringContent(str, Encoding.UTF8, "application/json"));
        }

        private async Task<T> RequestAsync<T>(Func<string, HttpContent, Task<HttpResponseMessage>> method, string url, HttpContent content)
        {
            var response = await RequestAsync(method, url, content);
            var obj = JsonConvert.DeserializeObject<T>(response);
            return obj;
        }

        private async Task<string> RequestAsync(Func<string, Task<HttpResponseMessage>> method, string url) =>
            await RequestAsync(async () => await method(url));

        private async Task<string> RequestAsync(Func<string, HttpContent, Task<HttpResponseMessage>> method, string url, HttpContent content) =>
            await RequestAsync(async () => await method(url, content));

        private async Task<string> RequestAsync(Func<Task<HttpResponseMessage>> method, bool logIn = true)
        {
            try
            {
                var response = await method();
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return content;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized && logIn)
                {
                    await LogInAsync();
                    return await RequestAsync(method, false);
                }
                else
                {
                    throw new ServerException(response, content); 
                }

            }
            catch (HttpRequestException ex)
            {
                throw new ServerException($"Erro ao se conectar ao servidor.", ex);
            }

        }
    }
}
