using Microsoft.EntityFrameworkCore.Migrations;

namespace MerendaIFCE.Sync.Migrations
{
    public partial class AddColumn_Confirmacao_Cancela : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Cancela",
                table: "Confirmacoes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cancela",
                table: "Confirmacoes");
        }
    }
}
