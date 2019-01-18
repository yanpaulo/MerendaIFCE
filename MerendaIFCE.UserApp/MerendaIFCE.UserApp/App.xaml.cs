using System;
using Xamarin.Forms;
using MerendaIFCE.UserApp.Views;
using Xamarin.Forms.Xaml;
using MerendaIFCE.UserApp.Views.Conta;
using MerendaIFCE.UserApp.Models;
using System.Linq;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace MerendaIFCE.UserApp
{
    public partial class App : Application
    {

        public static new App Current => Application.Current as App;

        public App()
        {
            InitializeComponent();
            MainPage = new StartupPage();
        }

        protected override void OnStart()
        {
            var db = AppDbContext.Instance;
            db.Initialize();

            usuario = db.GetUsuario();
            if (Usuario == null)
            {
                MainPage = new LoginPage();
            }
            else
            {
                MainPage = new RootPage();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
            Current.SavePropertiesAsync();
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }


    }
}
