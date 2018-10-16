using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MerendaIFCE.UserApp.Exceptions
{
    class ServerException : AppException
    {
        public ServerException()
        {
        }

        public ServerException(string message, HttpStatusCode statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        
        public HttpStatusCode StatusCode { get; private set; }

        public override async Task HandleAsync(Page page)
        {
            await page.DisplayAlert("Erro", $"Erro de servidor ({StatusCode}):\n{Message}", "Ok");
        }
    }
}
