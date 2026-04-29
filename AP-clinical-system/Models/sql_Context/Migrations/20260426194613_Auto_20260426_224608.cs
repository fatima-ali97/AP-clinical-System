using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP_clinical_system.Models.sql_Context.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20260426_224608 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "firt_name",
                table: "system_users",
                newName: "first_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "system_users",
                newName: "firt_name");
        }
    }
}
