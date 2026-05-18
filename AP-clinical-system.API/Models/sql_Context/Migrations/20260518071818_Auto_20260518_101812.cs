using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP_clinical_system.Models.sql_Context.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20260518_101812 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "medical_conditions",
                table: "patient_informations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "medical_conditions",
                table: "patient_informations");
        }
    }
}
