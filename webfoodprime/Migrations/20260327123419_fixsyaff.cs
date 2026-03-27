using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace webfoodprime.Migrations
{
    /// <inheritdoc />
    public partial class fixsyaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StaffId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Orders");
        }
    }
}
