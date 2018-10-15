using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MerendaIFCE.UserApp.Exceptions
{
    public class AppException : ApplicationException
    {
        public async Task HandleAsync(Page page)
        {
            await page.DisplayAlert("Erro", Message, "Ok");
        }
    }
}
