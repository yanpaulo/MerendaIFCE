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
using Yansoft.Rest;

namespace MerendaIFCE.Sync.Services
{
    class SyncWebService
    {
        private const string JsonContentType = "application/json";

        private RestHttpClient client;

        public SyncWebService()
        {
            client = new RestHttpClient
            {
#if DEMO
                BaseAddress = new Uri("http://almoco.yan-soft.com/api/")
#else
                BaseAddress = new Uri("http://localhost:7354/api/")
#endif
                ,
                ErrorHandler = HandleErrorAsync,
                Converter = new JsonRestConverter
                {
                    JsonSerializer = new JsonSerializer
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }
                }
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

        public async Task<HttpResponseMessage> HandleErrorAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await LogInAsync();
                return await client.SendAsync(request.Clone());
            }
            return response;
        }

        public async Task LogInAsync()
        {
            using (var db = new LocalDbContext())
            {
                Login login = new Login();
                App.Current.Settings.GetSection("SyncUser").Bind(login);
                var usuario = await client.RestPostAsync<Usuario>("Conta/Login", login);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", usuario.Token);

                db.Usuario.RemoveRange(db.Usuario.ToList());
                db.Add(usuario);
                db.SaveChanges();
            }

        }

        public async Task<IList<Inscricao>> GetInscricoesAsync(DateTimeOffset? ultimaAlteracao = null) =>
            await client.RestGetAsync<List<Inscricao>>($"Inscricoes?alteracao={ultimaAlteracao?.ToString("o")}");

        public async Task<IList<Confirmacao>> GetConfirmacoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            var list = await client.RestGetAsync<List<ConfirmacaoDTO>>($"Confirmacoes?alteracao={ultimaAlteracao?.ToString("o")}");
            return Mapper.Map<List<Confirmacao>>(list);
        }

        public async Task<IList<ConfirmacaoDTO>> PostConfirmacoesAsync(IEnumerable<Confirmacao> confirmacoes)
        {
            var list = Mapper.Map<List<ConfirmacaoDTO>>(confirmacoes);
            return await client.RestPostAsync<List<ConfirmacaoDTO>>("Confirmacoes", list);
        }
    }
}
