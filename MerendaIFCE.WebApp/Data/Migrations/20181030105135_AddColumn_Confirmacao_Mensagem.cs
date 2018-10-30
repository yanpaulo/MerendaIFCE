using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MerendaIFCE.WebApp.Data.Migrations
{
    public partial class AddColumn_Confirmacao_Mensagem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mensagem",
                table: "Confirmacoes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mensagem",
                table: "Confirmacoes");
        }
    }
}
