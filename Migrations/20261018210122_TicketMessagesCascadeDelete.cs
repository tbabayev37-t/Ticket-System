using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSistemi.Migrations
{
    /// <inheritdoc />
    public partial class TicketMessagesCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketMessages_AspNetUsers_AuthorId",
                table: "TicketMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketMessages_AspNetUsers_AuthorId",
                table: "TicketMessages",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketMessages_AspNetUsers_AuthorId",
                table: "TicketMessages");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketMessages_AspNetUsers_AuthorId",
                table: "TicketMessages",
                column: "AuthorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
