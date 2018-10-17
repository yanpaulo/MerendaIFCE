using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MerendaIFCE.UserApp.Models;

namespace MerendaIFCE.UserApp
{
    public partial class App : Application
    {
        private readonly AppDbContext db = AppDbContext.Instance;
        private Usuario usuario;

        public Usuario Usuario
        {
            get { return usuario; }
            set { SetUsuario(value); }
        }

        private void SetUsuario(Usuario usuario)
        {
            db.SetUsuario(usuario);
            this.usuario = usuario;
        }

        public void AddDia(InscricaoDia dia)
        {
            db.InsertDia(dia);
            Usuario.Inscricao.Dias.RemoveAll(d => d.InscricaoId == dia.InscricaoId && d.Dia == dia.Dia);
            Usuario.Inscricao.Dias.Add(dia);
        }

        public void RemoveDia(InscricaoDia dia)
        {
            db.DeleteDia(dia);
            Usuario.Inscricao.Dias.Remove(dia);
        }
    }
}
