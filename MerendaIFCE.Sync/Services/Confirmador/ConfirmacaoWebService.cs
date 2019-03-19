using MerendaIFCE.Sync.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yansoft.Rest;

namespace MerendaIFCE.Sync.Services.Confirmador
{
    public class ConfirmacaoWebService
    {
        private string token;
        private Refeicao refeicao;
        private RestHttpClient client = new RestHttpClient
        {
            BaseAddress = new Uri("http://intranet.maracanau.ifce.edu.br/ifce/ra/"),
            Deserializer = new StringOrObjectDeserializer()
        };

        private ConfirmacaoWebService()
        {
            client.ErrorHandler = HandleErrorAsync;
        }

        public static ConfirmacaoWebService Instance { get; private set; } = new ConfirmacaoWebService();


        public async Task<Aluno> GetAlunoAsync(string matricula)
        {
            var lista = await client.RestGetAsync<List<Aluno>>($"{matricula}/1");
            return lista.FirstOrDefault();
        }

#if MOCK
        public Task<Refeicao> GetRefeicaoAsync() => Task.FromResult(new Refeicao
        {
            Id = 1,
            Nome = "Sopa de olho com víboras",
            Data = DateTimeOffset.Now
        });

        public Task ConfirmaAsync(Confirmacao confirmacao, int retry = 1)
        {
            confirmacao.Mensagem = "Refeição confirmada.";
            return Task.CompletedTask;
        }
#else
        public async Task<Refeicao> GetRefeicaoAsync()
        {
            if (this.refeicao?.Data == App.Current.Today)
            {
                return this.refeicao;
            }

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("data", App.Current.Today.ToString("yyyy-MM-dd"))
            });

            var content = await client.RestPostAsync<string>("refeicao/filtrar", data);

            content = content
                .Replace(@"""[", "[")
                .Replace(@"]""", "]")
                .Replace(@"\""", "'");

            var refeicao = JsonConvert.DeserializeObject<List<Refeicao>>(content)?
                .FirstOrDefault();

            if (refeicao == null)
            {
                throw new InvalidOperationException("O servidor de confirmação não retornou refeições.");
            }

            return this.refeicao = refeicao;
        }

        public async Task ConfirmaAsync(Confirmacao confirmacao, int retry = 1)
        {
            var refeicao = await GetRefeicaoAsync();

            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("_token", token),
                new KeyValuePair<string, string>("data", App.Current.Today.ToString("yyyy-MM-dd")),
                new KeyValuePair<string, string>("identificacao", confirmacao.Inscricao.Matricula),
                new KeyValuePair<string, string>("refeicao", refeicao.Id.ToString()),
            });

            const string url = "refeicao/pedido";
            var content = await client.RestPostAsync<string>(url, data);

            try
            {
                confirmacao.Mensagem = GetValueOrContent(content, "id", "aviso-msg");
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"A URL ({client.BaseAddress}/{url}) não retornou o conteúdo esperado", ex);
            }
        }
#endif


        private string GetValueOrContent(string html, string attr, string nome)
        {
            var pattern = $@"<.*? {attr}=""{nome}"" value=""(.*?)"".*?>|<.*? {attr}=""{nome}"" value=""(.*?)"".*?>|<.*? {attr}=""{nome}"".*?>(.*?)</.*>";
            var match = Regex.Match(html, pattern, RegexOptions.Singleline);
            var value =
                match.Groups[1].Success ? match.Groups[1].Value :
                    match.Groups[2].Success ? match.Groups[2].Value :
                        match.Groups[3].Success ? match.Groups[3].Value.Trim() :
                            throw new InvalidOperationException($"Elemento com {attr}={nome} não encontrado.");

            return value;
        }

        private async Task AtualizaTokensAsync()
        {
            try
            {
                var content = await client.RestGetAsync<string>("");

                token = GetValueOrContent(content, "name", "_token");
                client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidOperationException($"A URL ({client.BaseAddress}) não retornou o conteúdo esperado", ex);
            }
        }

        private async Task<HttpResponseMessage> HandleErrorAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            {
                await AtualizaTokensAsync();
                return await client.SendAsync(request.Clone());
            }

            return response;
        }
    }
}
