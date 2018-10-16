using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MerendaIFCE.WebApp.Data.Migrations
{
    public partial class Relationship_Confirmacao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmacoes_InscricaoDia_InscricaoDiaId",
                table: "Confirmacoes");

            migrationBuilder.DropIndex(
                name: "IX_Confirmacoes_InscricaoDiaId",
                table: "Confirmacoes");

            migrationBuilder.DropColumn(
                name: "InscricaoDiaId",
                table: "Confirmacoes");

            migrationBuilder.AddColumn<int>(
                name: "InscricaoId",
                table: "Confirmacoes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UltimaModificacao",
                table: "Confirmacoes",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Confirmacoes_InscricaoId",
                table: "Confirmacoes",
                column: "InscricaoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmacoes_Inscricoes_InscricaoId",
                table: "Confirmacoes",
                column: "InscricaoId",
                principalTable: "Inscricoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Confirmacoes_Inscricoes_InscricaoId",
                table: "Confirmacoes");

            migrationBuilder.DropIndex(
                name: "IX_Confirmacoes_InscricaoId",
                table: "Confirmacoes");

            migrationBuilder.DropColumn(
                name: "InscricaoId",
                table: "Confirmacoes");

            migrationBuilder.DropColumn(
                name: "UltimaModificacao",
                table: "Confirmacoes");

            migrationBuilder.AddColumn<int>(
                name: "InscricaoDiaId",
                table: "Confirmacoes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Confirmacoes_InscricaoDiaId",
                table: "Confirmacoes",
                column: "InscricaoDiaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Confirmacoes_InscricaoDia_InscricaoDiaId",
                table: "Confirmacoes",
                column: "InscricaoDiaId",
                principalTable: "InscricaoDia",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
