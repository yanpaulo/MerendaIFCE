using MerendaIFCE.UserApp.Exceptions;
using MerendaIFCE.UserApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MerendaIFCE.UserApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RootPage : MasterDetailPage
    {
        public RootPage()
        {
            InitializeComponent();
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private async void MasterDetailPage_Appearing(object sender, EventArgs e)
        {
            var ws = new WebService();

            try
            {
                var canal = await DependencyService.Get<IPushService>().GetCanal();
                await ws.InscreveNotificacaoAsync(canal);
            }
            catch (ServerException ex)
            {
                await ex.HandleAsync(this);
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is RootPageMenuItem item))
                return;

            var page = (Page)Activator.CreateInstance(item.TargetType);
            page.Title = item.Title;

            Detail = new NavigationPage(page);
            IsPresented = false;

            MasterPage.ListView.SelectedItem = null;
        }

    }
}