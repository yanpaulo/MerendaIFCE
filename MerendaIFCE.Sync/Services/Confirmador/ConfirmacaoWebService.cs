using MerendaIFCE.Sync.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

        private async Task AtualizaTokensAsync()
        {
            var response = await client.GetAsync("");
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Erro ({response.StatusCode}:\r\n {content}");
            }

            try
            {
                token = GetValueOrContent(content, "name", "_token");
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<Aluno> GetAlunoAsync(string matricula)
        {
            var response = await client.GetAsync($"http://intranet.maracanau.ifce.edu.br/ifce/ra/{matricula}/1");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException($"Erro ({response.StatusCode}:\r\n {content}");
            }

            var lista = JsonConvert.DeserializeObject<List<Aluno>>(content);
            return lista.FirstOrDefault();
        }

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

            var response =await client.PostAsync("refeicao/filtrar", data);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException();
            }

            var refeicao = JsonConvert.DeserializeObject<List<Refeicao>>(content)?
                .FirstOrDefault();

            if (refeicao == null)
            {
                throw new ApplicationException("Servidor de confirmação não retornou refeições.");
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

            var response = await client.PostAsync("refeicao/pedido", data);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (retry > 0)
                {
                    await AtualizaTokensAsync();
                    await ConfirmaAsync(confirmacao, retry - 1);
                }
                else
                {
                    throw new ApplicationException($"Erro ({response.StatusCode}:\r\n {content}");
                }
            }

            try
            {
                confirmacao.Mensagem = GetValueOrContent(content, "id", "aviso-msg");
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
      

        private string GetValueOrContent(string html, string attr, string nome)
        {
            var pattern = $@"<.*? {attr}=""{nome}"" value=""(.*?)"".*?>|<.*? {attr}=""{nome}"" value=""(.*?)"".*?>|<.*? {attr}=""{nome}"".*?>(.*?)</.*>";
            var match = Regex.Match(html, pattern);
            var value =
                match.Groups[1].Success ? match.Groups[1].Value :
                    match.Groups[2].Success ? match.Groups[2].Value :
                        match.Groups[3].Success ? match.Groups[3].Value.Trim() :
                            throw new InvalidOperationException($"Elemento com {attr}={nome} não encontrado.");

            return value;
        }

    }
}
