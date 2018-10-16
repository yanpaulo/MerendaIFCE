using MerendaIFCE.UserApp.Exceptions;
using MerendaIFCE.UserApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public async Task<Usuario> CadastraAsync(Cadastro cadastro)
        {
            var content = new StringContent(JsonConvert.SerializeObject(cadastro), Encoding.UTF8, JsonContentType);
            var response = await client.PostAsync("Conta/Cadastro", content);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var usuario = JsonConvert.DeserializeObject<Usuario>(result);
                return usuario;
            }

            throw new ServerException(result, response.StatusCode);
        }

        public void Dispose()
        {
            
        }
    }
}
