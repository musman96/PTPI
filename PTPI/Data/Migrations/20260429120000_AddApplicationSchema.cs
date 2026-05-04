using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTPI.Data.Migrations
{
    
    public partial class AddApplicationSchema : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_closed",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_closed",
                table: "Accounts");
        }
    }
}
