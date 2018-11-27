using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MerendaIFCE.UserApp.Views.Main
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConfirmacoesView : ContentPage
	{
        private ConfirmacoesViewModel viewModel;
		public ConfirmacoesView ()
		{
			InitializeComponent();
            BindingContext = viewModel = new ConfirmacoesViewModel();
		}

        private async void ContentPage_Appearing(object sender, EventArgs e)
        {
            await viewModel.LoadAsync();
        }
    }
}