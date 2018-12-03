using MerendaIFCE.UserApp.Models;
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

        private async void CancelaSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            var element = sender as Switch;
            if (element.IsEnabled)
            {
                var item = element.BindingContext as Confirmacao;

                element.IsEnabled = false;
                try
                {
                    await viewModel.UpdateCancelamento();
                }
                finally
                {
                    element.IsEnabled = true;
                } 
            }
        }
    }
}