using System;
using Xamarin.Forms;
using MerendaIFCE.UserApp.Views;
using Xamarin.Forms.Xaml;
using MerendaIFCE.UserApp.Views.Conta;
using MerendaIFCE.UserApp.Models;
using Microsoft.EntityFrameworkCore;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace MerendaIFCE.UserApp
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();


            MainPage = new CadastroPage();
		}

		protected override void OnStart ()
		{
            using (var db = new UserAppDbContext())
            {
                db.Database.Migrate();
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
