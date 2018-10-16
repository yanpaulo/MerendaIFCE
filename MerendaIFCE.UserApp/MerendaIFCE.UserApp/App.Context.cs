using System;
using Xamarin.Forms;
using MerendaIFCE.UserApp.Views;
using Xamarin.Forms.Xaml;
using MerendaIFCE.UserApp.Views.Conta;
using MerendaIFCE.UserApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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

        private void SetUsuario(Usuario value)
        {
            using (var db = new UserAppDbContext())
            {
                if (db.Usuarios.Any())
                {
                    db.Inscricoes.RemoveRange(db.Inscricoes.ToList());
                    db.Usuarios.RemoveRange(db.Usuarios.ToList());
                }
                if (value != null)
                {
                    db.Usuarios.Add(value); 
                }
                db.SaveChanges();
                usuario = value;
            }
        }
    }
}
