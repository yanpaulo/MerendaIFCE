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
        private HttpClient client = new HttpClient
        {
            BaseAddress = new Uri("http://intranet.maracanau.ifce.edu.br/ifce/ra/")
        };
        private ConfirmacaoWebService() { }

        public static ConfirmacaoWebService Instance { get; private set; } = new ConfirmacaoWebService();


        public async Task<Aluno> GetAlunoAsync(string matricula)
        {
            var content = await RequestAsync(client.GetAsync, $"{matricula}/1");

            var lista = JsonConvert.DeserializeObject<List<Aluno>>(content);
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

            var content = await RequestAsync(client.PostAsync, "refeicao/filtrar", data);

            content = content
                .Replace(@"""[", "[")
                .Replace(@"]""", "]")
                .Replace(@"\""", "'");

            var refeicao = JsonConvert.DeserializeObject<List<Refeicao>>(content)?
                .FirstOrDefault();

            if (refeicao == null)
            {
                throw new ServerException("O servidor de confirmação não retornou refeições.");
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
            var content = await RequestAsync(client.PostAsync, url, data);

            try
            {
                confirmacao.Mensagem = GetValueOrContent(content, "id", "aviso-msg");
            }
            catch (InvalidOperationException ex)
            {
                throw new ServerException($"A URL ({client.BaseAddress}/{url}) não retornou o conteúdo esperado", ex);
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
                var content = await RequestAsync(async () => await client.GetAsync(""), false);

                token = GetValueOrContent(content, "name", "_token");
                client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", token);
            }
            catch (InvalidOperationException ex)
            {
                throw new RestException($"A URL ({client.BaseAddress}) não retornou o conteúdo esperado", ex);
            }
        }


        private async Task<string> RequestAsync(Func<string, Task<HttpResponseMessage>> method, string url) =>
            await RequestAsync(async () => await method(url));

        private async Task<string> RequestAsync(Func<string, HttpContent, Task<HttpResponseMessage>> method, string url, HttpContent content) =>
            await RequestAsync(async () => await method(url, content));

        private async Task<string> RequestAsync(Func<Task<HttpResponseMessage>> method, bool refreshToken = true)
        {
            try
            {
                var response = await method();
                var content = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    return content;
                }
                if (refreshToken)
                {
                    await AtualizaTokensAsync();
                    await RequestAsync(method, false);
                }
                throw new RestException(response, content);

            }
            catch (HttpRequestException ex)
            {
                throw new RestException($"Erro ao se conectar ao servidor {client.BaseAddress}", ex);
            }

        }

    }
}
