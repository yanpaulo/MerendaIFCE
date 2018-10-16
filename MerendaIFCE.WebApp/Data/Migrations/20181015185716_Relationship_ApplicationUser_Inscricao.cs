using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MerendaIFCE.WebApp.Data.Migrations
{
    public partial class Relationship_ApplicationUser_Inscricao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Inscricoes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inscricoes_ApplicationUserId",
                table: "Inscricoes",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Inscricoes_AspNetUsers_ApplicationUserId",
                table: "Inscricoes",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inscricoes_AspNetUsers_ApplicationUserId",
                table: "Inscricoes");

            migrationBuilder.DropIndex(
                name: "IX_Inscricoes_ApplicationUserId",
                table: "Inscricoes");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Inscricoes");
        }
    }
}
