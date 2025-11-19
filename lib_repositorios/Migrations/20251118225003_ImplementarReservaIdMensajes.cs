using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace lib_repositorios.Migrations
{
    /// <inheritdoc />
    public partial class ImplementarReservaIdMensajes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReservaId",
                table: "Mensajes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_ReservaId",
                table: "Mensajes",
                column: "ReservaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Mensajes_Reservas_ReservaId",
                table: "Mensajes",
                column: "ReservaId",
                principalTable: "Reservas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mensajes_Reservas_ReservaId",
                table: "Mensajes");

            migrationBuilder.DropIndex(
                name: "IX_Mensajes_ReservaId",
                table: "Mensajes");

            migrationBuilder.DropColumn(
                name: "ReservaId",
                table: "Mensajes");
        }
    }
}
