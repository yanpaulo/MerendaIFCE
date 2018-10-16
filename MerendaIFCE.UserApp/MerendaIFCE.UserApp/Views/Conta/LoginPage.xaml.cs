using MerendaIFCE.UserApp.Exceptions;
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
	public partial class LoginPage : ContentPage
	{
        private LoginViewModel viewModel;

		public LoginPage ()
		{
            BindingContext = viewModel = new LoginViewModel();
			InitializeComponent ();
		}

        private async void LoginButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                await viewModel.LoginAsync();
                App.Current.MainPage = new RootPage();
            }
            catch (AppException ex)
            {
                await ex.HandleAsync(this);
            }
        }

        private void CadastroButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new NavigationPage(new CadastroPage()));
        }
    }
}