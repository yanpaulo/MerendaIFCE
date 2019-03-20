using MerendaIFCE.UserApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MerendaIFCE.UserApp.Views.Conta
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogoutPage : ContentPage
	{
		public LogoutPage ()
		{
			InitializeComponent();
		}

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            AppDbContext.Instance.LimpaDados();
            await Task.Delay(500);
            App.Current.MainPage = new LoginPage();
        }
    }
}