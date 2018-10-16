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
    public partial class CadastroPage : ContentPage
    {
        private RegisterPageViewModel viewModel;
        public CadastroPage()
        {
            BindingContext = viewModel = new RegisterPageViewModel();
            InitializeComponent();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                await viewModel.CadastraAsync();
                var root = new RootPage();
                App.Current.MainPage = root;
            }
            catch (AppException ex)
            {
                await ex.HandleAsync(this);
            }
        }
    }
}