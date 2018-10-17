using MerendaIFCE.UserApp.Exceptions;
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
	public partial class SelecaoDiasPage : ContentPage
	{
        private SelecaoDiasViewModel viewModel;
		public SelecaoDiasPage ()
		{
            BindingContext = viewModel = new SelecaoDiasViewModel();
			InitializeComponent();
		}

        private async void Switch_Toggled(object sender, ToggledEventArgs e)
        {
            var item = ((Switch)sender).BindingContext as SelecaoDiasViewModelItem;
            try
            {
                await item.OnChangedAsync();
            }
            catch (ServerException)
            {

            }
        }
    }
}