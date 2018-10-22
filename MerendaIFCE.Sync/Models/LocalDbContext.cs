using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerendaIFCE.Sync.Models
{
    public class LocalDbContext : DbContext
    {
        public DbSet<Inscricao> Inscricoes { get; set; }

        public DbSet<InscricaoDia> InscricaoDias { get; set; }

        public DbSet<Confirmacao> Confirmacoes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=data.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Inscricao>()
                .HasMany(i => i.Dias)
                .WithOne(d => d.Inscricao)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inscricao>()
                .HasMany(i => i.Confirmacoes)
                .WithOne(c => c.Inscricao)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
