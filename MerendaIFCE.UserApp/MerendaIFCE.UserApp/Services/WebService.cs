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

        public async Task<Inscricao> PutInscricaoDiasAsync(Inscricao inscricao)
        {
            return await EnviaAsync<Inscricao>(inscricao, $"Inscricoes/{inscricao?.Id}", client.PutAsync);
        }

        public async Task<T> EnviaAsync<T>(object item, string url, Func<string, HttpContent, Task<HttpResponseMessage>> method )
        {
            var content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, JsonContentType);
            var response = await method(url, content);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var ret = JsonConvert.DeserializeObject<T>(result);
                return ret;
            }

            throw new ServerException(result, response.StatusCode);
        }

        public void Dispose()
        {
            
        }
    }
}
