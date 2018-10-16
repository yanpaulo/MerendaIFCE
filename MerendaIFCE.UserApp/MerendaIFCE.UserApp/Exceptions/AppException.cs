using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MerendaIFCE.UserApp.Exceptions
{
    public class AppException : ApplicationException
    {
        public AppException()
        {
        }

        public AppException(string message) : base(message)
        {
        }

        public AppException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AppException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public virtual async Task HandleAsync(Page page)
        {
            await page.DisplayAlert("Erro", Message, "Ok");
        }
    }
}
