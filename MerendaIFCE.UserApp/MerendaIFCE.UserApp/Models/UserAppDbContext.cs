using MerendaIFCE.UserApp.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MerendaIFCE.UserApp.Models
{
    public class UserAppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Inscricao> Inscricoes { get; set; }

        public DbSet<InscricaoDia> InscricoesDia { get; set; }

        public DbSet<Confirmacao> Confirmacoes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Inscricao)
                .WithOne()
                .HasForeignKey<Inscricao>("UsuarioId");

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var path = DependencyService.Get<IPathService>().GetLocalFilePath("data.db");
            optionsBuilder.UseSqlite($"Data Source={path}");

            base.OnConfiguring(optionsBuilder);
        }
    }
}
