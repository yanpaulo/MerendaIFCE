using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MerendaIFCE.UserApp.Models;

namespace MerendaIFCE.UserApp
{
    public partial class App : Application
    {
        private Usuario usuario;

        public Usuario Usuario
        {
            get { return usuario; }
            set { SetUsuario(value); }
        }

        private void SetUsuario(Usuario usuario)
        {
            var db = AppDbContext.Instance;
            db.SetUsuario(usuario);
            this.usuario = usuario;
        }
    }
}
