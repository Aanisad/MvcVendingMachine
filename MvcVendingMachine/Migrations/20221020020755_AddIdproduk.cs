using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcVendingMachine.Migrations
{
    public partial class AddIdproduk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Idproduk",
                table: "Image",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Machine_Idproduk",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_Idproduk",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "Idproduk",
                table: "Image");
        }
    }
}
