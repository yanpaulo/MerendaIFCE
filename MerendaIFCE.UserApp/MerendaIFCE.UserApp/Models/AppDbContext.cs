using MerendaIFCE.UserApp.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
namespace MerendaIFCE.UserApp.Models
{
    public class AppDbContext
    {
        private SQLiteConnection db;

        public static AppDbContext Instance { get; private set; } = new AppDbContext();
        private AppDbContext() { }

        public void Initialize()
        {
            db = new SQLiteConnection(DependencyService.Get<IPathService>().GetLocalFilePath("data.db"));
            db.CreateTable<Usuario>();
            db.CreateTable<Inscricao>();
            db.CreateTable<InscricaoDia>();
            db.CreateTable<Confirmacao>();

            var min = DateTimeOffset.Now - TimeSpan.FromDays(60);
            db.Table<Confirmacao>().Delete(c => c.Dia < min);
        }

        public Usuario GetUsuario()
        {
            var usuario = db.Table<Usuario>().SingleOrDefault();
            if (usuario != null)
            {
                usuario.Inscricao = GetInscricao(); 
            }
            return usuario;
        }

        public void SetUsuario(Usuario u)
        {
            db.DeleteAll<Usuario>();
            db.DeleteAll<Inscricao>();
            db.Insert(u);

            db.Table<Confirmacao>().Delete(c => c.InscricaoId != u.Inscricao.Id);
            db.Insert(u.Inscricao);
        }

        public Inscricao GetInscricao()
        {
            var inscricao = db.Table<Inscricao>().SingleOrDefault();
            if (inscricao != null)
            {
                inscricao.Dias = db.Table<InscricaoDia>().ToList();
                inscricao.Confirmacoes = db.Table<Confirmacao>().ToList();
            }
            return inscricao;
        }

        public void SetInscricao(Inscricao inscricao)
        {
            db.Table<Inscricao>().Delete();
            db.Insert(inscricao);
            if (inscricao.Dias != null)
            {
                db.InsertAll(inscricao.Dias);
            }
        }

        public void InsertDia(InscricaoDia dia)
        {
            db.Table<InscricaoDia>().Delete(i => i.InscricaoId == i.InscricaoId && i.Dia == dia.Dia);
            db.Insert(dia);
        }

        public void DeleteDia(InscricaoDia dia)
        {
            db.Delete(dia);
        }





    }
}
