using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MerendaIFCE.Sync.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inscricoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Matricula = table.Column<string>(nullable: false),
                    UltimaModificacao = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscricoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Confirmacoes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IdRemoto = table.Column<int>(nullable: true),
                    UltimaModificacao = table.Column<DateTimeOffset>(nullable: true),
                    Dia = table.Column<DateTimeOffset>(nullable: false),
                    StatusConfirmacao = table.Column<int>(nullable: false),
                    StatusSincronia = table.Column<int>(nullable: false),
                    InscricaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Confirmacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Confirmacoes_Inscricoes_InscricaoId",
                        column: x => x.InscricaoId,
                        principalTable: "Inscricoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InscricaoDias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Dia = table.Column<int>(nullable: false),
                    InscricaoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InscricaoDias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InscricaoDias_Inscricoes_InscricaoId",
                        column: x => x.InscricaoId,
                        principalTable: "Inscricoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Confirmacoes_InscricaoId",
                table: "Confirmacoes",
                column: "InscricaoId");

            migrationBuilder.CreateIndex(
                name: "IX_InscricaoDias_InscricaoId",
                table: "InscricaoDias",
                column: "InscricaoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Confirmacoes");

            migrationBuilder.DropTable(
                name: "InscricaoDias");

            migrationBuilder.DropTable(
                name: "Inscricoes");
        }
    }
}
