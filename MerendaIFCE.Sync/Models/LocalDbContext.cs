using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MerendaIFCE.Sync.Models
{
    public class LocalDbContext : DbContext
    {
        public DbSet<Inscricao> Inscricoes { get; set; }

        public DbSet<InscricaoDia> InscricaoDias { get; set; }

        public DbSet<Confirmacao> Confirmacoes { get; set; }

        public DbSet<Usuario> Usuario { get; set; }

        public void UpdateInscricao(Inscricao inscricao)
        {
            if (Inscricoes.Include(i => i.Dias).SingleOrDefault(i => i.Id == inscricao.Id) is Inscricao local)
            {
                RemoveRange(local.Dias);

                Entry(local).CurrentValues.SetValues(inscricao);
                Entry(local).Collection(l => l.Dias).CurrentValue = inscricao.Dias;
            }
            else
            {
                Add(inscricao);
            }
        }

        public void UpdateConfirmacao(Confirmacao confirmacao)
        {
            if (confirmacao.Id != 0 && confirmacao.IdRemoto == 0)
            {
                throw new InvalidOperationException("Id local é diferente de zero mas id remoto é zero. Que porra é essa??");
            }
            if (Confirmacoes.SingleOrDefault(i => i.IdRemoto == confirmacao.IdRemoto) is Confirmacao local)
            {
                confirmacao.Id = local.Id;
                Entry(local).CurrentValues.SetValues(confirmacao);
            }
            else
            {
                Add(confirmacao);
            }
        }

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
