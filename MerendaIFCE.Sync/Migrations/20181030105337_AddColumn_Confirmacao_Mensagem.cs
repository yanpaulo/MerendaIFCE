using Microsoft.EntityFrameworkCore.Migrations;

namespace MerendaIFCE.Sync.Migrations
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
