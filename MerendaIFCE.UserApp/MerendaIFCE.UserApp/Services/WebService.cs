using MerendaIFCE.UserApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Services
{
    public class WebService
    {
        private const string JsonContentType = "application/json";
        private HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:7354/api/")
        };

        public async Task CadastraAsync(Cadastro cadastro)
        {
            var content = new StringContent(JsonConvert.SerializeObject(cadastro), Encoding.UTF8, JsonContentType);
            var response = await client.PostAsync("Register", content);
            if (!response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                
            }
        }
    }
}
