using MerendaIFCE.UserApp.Exceptions;
using MerendaIFCE.UserApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Yansoft.Rest;

namespace MerendaIFCE.UserApp.Services
{
    public class WebService : IDisposable
    {
        private const string JsonContentType = "application/json";
        private RestHttpClient client = new RestHttpClient
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
            await client.RestPostAsync<Usuario>("Conta/Login", login);

        public async Task<Usuario> CadastraAsync(Cadastro cadastro) => 
            await client.RestPostAsync<Usuario>("Conta/Cadastro", cadastro);

        public async Task<InscricaoDia> PostDiaAsync(InscricaoDia dia) => 
            await client.RestPostAsync<InscricaoDia>($"Inscricoes/{dia.InscricaoId}/Dias", dia);

        public async Task DeleteDiaAsync(InscricaoDia dia) => 
            await client.RestDeleteAsync<string>($"Inscricoes/{dia.InscricaoId}/Dias/{dia.Id}");

        public async Task InscreveNotificacaoAsync(CanalPush canal) => 
            await client.RestPostAsync<string>("Notificacoes/Inscreve", canal);

        public async Task<List<Confirmacao>> GetConfirmacoesAsync(DateTimeOffset? ultimaAlteracao = null)
        {
            string url = $"Inscricoes/Confirmacoes?alteracao={(ultimaAlteracao.HasValue ? Uri.EscapeDataString(ultimaAlteracao?.ToString("o")) : null)}";
            return await client.RestGetAsync<List<Confirmacao>>(url);
        }

        public async Task<Confirmacao> PutConfirmacaoAsync(Confirmacao confirmacao) => 
            await client.RestPutAsync<Confirmacao>($"Inscricoes/Confirmacoes/{confirmacao.Id}", confirmacao);
        
        public void Dispose()
        {

        }
    }
}
