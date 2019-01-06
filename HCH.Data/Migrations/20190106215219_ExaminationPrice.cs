using Microsoft.EntityFrameworkCore.Migrations;

namespace HCH.Data.Migrations
{
    public partial class ExaminationPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Examinations",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Examinations");
        }
    }
}
