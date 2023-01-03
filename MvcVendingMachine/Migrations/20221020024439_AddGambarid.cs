using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcVendingMachine.Migrations
{
    public partial class AddGambarid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Machine_Idproduk",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_Idproduk",
                table: "Image");

            migrationBuilder.AddColumn<int>(
                name: "GambarId",
                table: "Machine",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GambarId",
                table: "Machine");

            migrationBuilder.CreateIndex(
                name: "IX_Image_Idproduk",
                table: "Image",
                column: "Idproduk");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Machine_Idproduk",
                table: "Image",
                column: "Idproduk",
                principalTable: "Machine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
