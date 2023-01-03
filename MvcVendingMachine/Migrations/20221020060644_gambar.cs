using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcVendingMachine.Migrations
{
    public partial class gambar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GambarId",
                table: "Machine");

            migrationBuilder.DropColumn(
                name: "Idproduk",
                table: "Image");

            migrationBuilder.AddColumn<int>(
                name: "machineId",
                table: "Image",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Image_machineId",
                table: "Image",
                column: "machineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Image_Machine_machineId",
                table: "Image",
                column: "machineId",
                principalTable: "Machine",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Image_Machine_machineId",
                table: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Image_machineId",
                table: "Image");

            migrationBuilder.DropColumn(
                name: "machineId",
                table: "Image");

            migrationBuilder.AddColumn<int>(
                name: "GambarId",
                table: "Machine",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Idproduk",
                table: "Image",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
