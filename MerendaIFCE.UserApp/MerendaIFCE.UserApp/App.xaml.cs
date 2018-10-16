using System;
using Xamarin.Forms;
using MerendaIFCE.UserApp.Views;
using Xamarin.Forms.Xaml;
using MerendaIFCE.UserApp.Views.Conta;
using MerendaIFCE.UserApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace MerendaIFCE.UserApp
{
	public partial class App : Application
	{

        public static new App Current => Application.Current as App;

        public App ()
		{
            InitializeComponent();
            MainPage = new RootPage();
        }

		protected override void OnStart ()
		{
            using (var db = new UserAppDbContext())
            {
                db.Database.EnsureCreated();
                usuario = db.Usuarios.SingleOrDefault();
                if (Usuario == null)
                {
                    MainPage = new LoginPage();
                }
                else
                {
                    MainPage = new RootPage();
                }
            }
        }

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

        
    }
}
