using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MvcVendingMachine.Migrations
{
    public partial class Pembayaran : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        
            {
                migrationBuilder.CreateTable(
                    name: "Pembayaran",
                    columns: table => new
                    {
                        Id = table.Column<int>(type: "int", nullable: false)
                            .Annotation("SqlServer:Identity", "1, 1"),
                        nominal = table.Column<decimal>(type: "decimal", nullable: false)
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Pembayaran", x => x.Id);
                    });
            }
        

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
