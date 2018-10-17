using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MerendaIFCE.WebApp.Data.Migrations
{
    public partial class DbSet_InscricaoDias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InscricaoDia_Inscricoes_InscricaoId",
                table: "InscricaoDia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InscricaoDia",
                table: "InscricaoDia");

            migrationBuilder.RenameTable(
                name: "InscricaoDia",
                newName: "InscricaoDias");

            migrationBuilder.RenameIndex(
                name: "IX_InscricaoDia_InscricaoId",
                table: "InscricaoDias",
                newName: "IX_InscricaoDias_InscricaoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InscricaoDias",
                table: "InscricaoDias",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InscricaoDias_Inscricoes_InscricaoId",
                table: "InscricaoDias",
                column: "InscricaoId",
                principalTable: "Inscricoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InscricaoDias_Inscricoes_InscricaoId",
                table: "InscricaoDias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InscricaoDias",
                table: "InscricaoDias");

            migrationBuilder.RenameTable(
                name: "InscricaoDias",
                newName: "InscricaoDia");

            migrationBuilder.RenameIndex(
                name: "IX_InscricaoDias_InscricaoId",
                table: "InscricaoDia",
                newName: "IX_InscricaoDia_InscricaoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InscricaoDia",
                table: "InscricaoDia",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InscricaoDia_Inscricoes_InscricaoId",
                table: "InscricaoDia",
                column: "InscricaoId",
                principalTable: "Inscricoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
