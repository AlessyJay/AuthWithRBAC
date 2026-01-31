using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace auth_jwt_rbac.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Roles",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "USER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "users");
        }
    }
}
