﻿// <auto-generated />
using System;
using MerendaIFCE.Sync.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MerendaIFCE.Sync.Migrations
{
    [DbContext(typeof(LocalDbContext))]
    [Migration("20181203133703_AddColumn_Confirmacao_Cancela")]
    partial class AddColumn_Confirmacao_Cancela
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("MerendaIFCE.Sync.Models.Confirmacao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Cancela");

                    b.Property<DateTimeOffset>("Dia");

                    b.Property<int?>("IdRemoto");

                    b.Property<int>("InscricaoId");

                    b.Property<string>("Mensagem");

                    b.Property<int>("StatusConfirmacao");

                    b.Property<int>("StatusSincronia");

                    b.Property<DateTimeOffset?>("UltimaModificacao");

                    b.HasKey("Id");

                    b.HasIndex("InscricaoId");

                    b.ToTable("Confirmacoes");
                });

            modelBuilder.Entity("MerendaIFCE.Sync.Models.Inscricao", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("Matricula")
                        .IsRequired();

                    b.Property<DateTimeOffset>("UltimaModificacao");

                    b.HasKey("Id");

                    b.ToTable("Inscricoes");
                });

            modelBuilder.Entity("MerendaIFCE.Sync.Models.InscricaoDia", b =>
                {
                    b.Property<int>("Id");

                    b.Property<int>("Dia");

                    b.Property<int>("InscricaoId");

                    b.HasKey("Id");

                    b.HasIndex("InscricaoId");

                    b.ToTable("InscricaoDias");
                });

            modelBuilder.Entity("MerendaIFCE.Sync.Models.Usuario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Login");

                    b.Property<string>("Token");

                    b.HasKey("Id");

                    b.ToTable("Usuario");
                });

            modelBuilder.Entity("MerendaIFCE.Sync.Models.Confirmacao", b =>
                {
                    b.HasOne("MerendaIFCE.Sync.Models.Inscricao", "Inscricao")
                        .WithMany("Confirmacoes")
                        .HasForeignKey("InscricaoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MerendaIFCE.Sync.Models.InscricaoDia", b =>
                {
                    b.HasOne("MerendaIFCE.Sync.Models.Inscricao", "Inscricao")
                        .WithMany("Dias")
                        .HasForeignKey("InscricaoId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
