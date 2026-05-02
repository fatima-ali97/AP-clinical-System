using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP_clinical_system.Models.sql_Context.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20260421_235438 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "time_slot",
                table: "appointments",
                newName: "appointment_time_slot");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "appointments",
                newName: "appointment_status");

            migrationBuilder.AddColumn<string>(
                name: "notification_no",
                table: "notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "record_no",
                table: "doctor_specializations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "record_no",
                table: "doctor_schedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "record_no",
                table: "doctor_leaves",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "field_name",
                table: "autonumbers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "notification_no",
                table: "notifications");

            migrationBuilder.DropColumn(
                name: "record_no",
                table: "doctor_specializations");

            migrationBuilder.DropColumn(
                name: "record_no",
                table: "doctor_schedules");

            migrationBuilder.DropColumn(
                name: "record_no",
                table: "doctor_leaves");

            migrationBuilder.DropColumn(
                name: "field_name",
                table: "autonumbers");

            migrationBuilder.RenameColumn(
                name: "appointment_time_slot",
                table: "appointments",
                newName: "time_slot");

            migrationBuilder.RenameColumn(
                name: "appointment_status",
                table: "appointments",
                newName: "status");
        }
    }
}
