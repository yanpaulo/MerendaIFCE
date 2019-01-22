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
#if DEMO 
            BaseAddress = new Uri("http://almoco.yan-soft.com/api/")
#elif AVD
            BaseAddress = new Uri("http://10.0.2.2:7354/api/")
#else
            BaseAddress = new Uri("http://localhost:7354/api/")
#endif
        };

        public WebService()
        {
            var usuario = App.Current.Usuario;
            if (usuario != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);
            }
        }

        public async Task<Usuario> LoginAsync(Login login) => 
            await RequestAsync<Usuario>(client.PostAsync, "Conta/Login", login);

        public async Task<Usuario> CadastraAsync(Cadastro cadastro) => 
            await RequestAsync<Usuario>(client.PostAsync, "Conta/Cadastro", cadastro);

        public async Task<InscricaoDia> PostDiaAsync(InscricaoDia dia) => 
            await RequestAsync<InscricaoDia>(client.PostAsync, $"Inscricoes/{dia.InscricaoId}/Dias", dia);

        public async Task DeleteDiaAsync(InscricaoDia dia) => 
            await RequestAsync(client.DeleteAsync, $"Inscricoes/{dia.InscricaoId}/Dias/{dia.Id}");

        public async Task InscreveNotificacaoAsync(CanalPush canal)
        {
            await RequestAsync<string>(client.PostAsync, "Notificacoes/Inscreve", canal);
        }

        public async Task<List<Confirmacao>> GetConfirmacoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            string url = $"Inscricoes/Confirmacoes?alteracao={(ultimaAlteracao.HasValue ? Uri.EscapeDataString(ultimaAlteracao?.ToString("o")) : null)}";
            return JsonConvert.DeserializeObject<List<Confirmacao>>(await RequestAsync(client.GetAsync, url));
        }

        public async Task<Confirmacao> PutConfirmacaoAsync(Confirmacao confirmacao) => 
            await RequestAsync<Confirmacao>(client.PutAsync, $"Inscricoes/Confirmacoes/{confirmacao.Id}", confirmacao);

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
