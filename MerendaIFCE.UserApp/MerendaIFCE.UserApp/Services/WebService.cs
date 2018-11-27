using MerendaIFCE.UserApp.Exceptions;
using MerendaIFCE.UserApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Services
{
    public class WebService : IDisposable
    {
        private const string JsonContentType = "application/json";
        private HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:7354/api/")
        };

        public WebService()
        {
            var usuario = App.Current.Usuario;
            if (usuario != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);
            }
        }

        public async Task<Usuario> LoginAsync(Login login)
        {
            return await EnviaAsync<Usuario>(login, "Conta/Login", client.PostAsync);
        }

        public async Task<Usuario> CadastraAsync(Cadastro cadastro)
        {
            return await EnviaAsync<Usuario>(cadastro, "Conta/Cadastro", client.PostAsync);
        }

        public async Task<InscricaoDia> PostDiaAsync(InscricaoDia dia)
        {
            return await EnviaAsync<InscricaoDia>(dia, $"Inscricoes/{dia.InscricaoId}/Dias", client.PostAsync);
        }

        public async Task DeleteDiaAsync(InscricaoDia dia)
        {
            await EnviaAsync<InscricaoDia>(dia, $"Inscricoes/{dia.InscricaoId}/Dias/{dia.Id}", async (url, content) => await client.DeleteAsync(url));
        }

        public async Task InscreveNotificacaoAsync(CanalPush canal)
        {
            await EnviaAsync(canal, "Notificacoes/Inscreve", client.PostAsync);
        }

        public async Task<List<Confirmacao>> GetConfirmacoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            string url = $"Inscricoes/Confirmacoes?alteracao={(ultimaAlteracao.HasValue ? Uri.EscapeDataString(ultimaAlteracao?.ToString("o")) : null)}";
            return JsonConvert.DeserializeObject<List<Confirmacao>>(await RequestAsync(client.GetAsync, url));
        }

        #region LIXO
        public async Task EnviaAsync(object item, string url, Func<string, HttpContent, Task<HttpResponseMessage>> method)
        {
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, JsonContentType);
            var response = await method(url, content);
            var result = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ServerException(response, result);
            }
        }

        public async Task<T> EnviaAsync<T>(object item, string url, Func<string, HttpContent, Task<HttpResponseMessage>> method)
        {
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, JsonContentType);
            var response = await method(url, content);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var ret = JsonConvert.DeserializeObject<T>(result);
                return ret;
            }

            throw new ServerException(response, result);
        } 
        #endregion

        private async Task<T> RequestAsync<T>(Func<string, Task<HttpResponseMessage>> method, string url)
        {
            var response = await RequestAsync(method, url);
            var obj = JsonConvert.DeserializeObject<T>(response);
            return obj;
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

        private async Task<string> RequestAsync(Func<Task<HttpResponseMessage>> method)
        {
            try
            {
                var response = await method();
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return content;
                }
                throw new ServerException(response, content);

            }
            catch (HttpRequestException ex)
            {
                throw new ServerException($"Erro ao se conectar ao servidor.", ex);
            }

        }

        public void Dispose()
        {

        }
    }
}
