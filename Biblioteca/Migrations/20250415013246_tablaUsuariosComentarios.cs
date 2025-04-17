using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteca.Migrations
{
    /// <inheritdoc />
    public partial class tablaUsuariosComentarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsuarioId",
                table: "comentarios",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_UsuarioId",
                table: "comentarios",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_comentarios_AspNetUsers_UsuarioId",
                table: "comentarios",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comentarios_AspNetUsers_UsuarioId",
                table: "comentarios");

            migrationBuilder.DropIndex(
                name: "IX_comentarios_UsuarioId",
                table: "comentarios");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "comentarios");
        }
    }
}
