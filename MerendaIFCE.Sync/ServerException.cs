using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace MerendaIFCE.Sync
{
    public class ServerException : Exception
    {
        public string Content { get; set; }

        public HttpResponseMessage Response { get; set; }

        public ServerException(HttpResponseMessage response, string content) : base($"Erro ao solicitar URL ({response.RequestMessage.RequestUri})")
        {
            Response = response;
            Content = content;
        }

        public ServerException(string message) : base(message)
        {
        }

        public ServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
