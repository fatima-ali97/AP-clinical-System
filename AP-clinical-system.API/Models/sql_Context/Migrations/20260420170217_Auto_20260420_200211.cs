using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AP_clinical_system.Models.sql_Context.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20260420_200211 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    appointment_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    appointment_reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    patient_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    patient_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    doctor_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctor_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    receptionist_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    receptionist_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    specialization_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    specialization_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    date = table.Column<DateOnly>(type: "date", nullable: false),
                    time_slot = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "autonumbers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    entity_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_number = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_autonumbers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "clinic_manager_informations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    record_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    job_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    system_user_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    system_user_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_clinic_manager_informations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctor_information_specialization_mtms",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    record_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    doctor_information_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctor_information_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    doctor_specialization_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctor_specialization_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_information_specialization_mtms", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctor_informations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    record_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    job_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    system_user_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    system_user_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_informations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctor_leaves",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    doctor_information_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctor_information_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    leave_type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_leaves", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctor_schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    doctor_information_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctor_information_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sunday_start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sunday_end = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    monday_start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    monday_end = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tuesday_start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    tuesday_end = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wednesday_start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    wednesday_end = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thursday_start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    thursday_end = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    friday_start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    friday_end = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    saturday_start = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    saturday_end = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_schedules", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "doctor_specializations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    specialization_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    specialization_details = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_doctor_specializations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    system_user_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    system_user_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_read = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notifications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "patient_informations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    record_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    system_user_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    system_user_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_patient_informations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "prescriptions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    prescription_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    medicine_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    medicine_dosage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    medicine_frequency_duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    appointment_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    appointment_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    patient_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    patient_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    doctor_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    doctor_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prescriptions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "receptionist_informations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    record_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    job_title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    system_user_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    system_user_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_receptionist_informations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "system_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    user_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cpr = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    firt_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    last_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    phone_number = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hashed_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    user_role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_system_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "visit_records",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    inactive = table.Column<bool>(type: "bit", nullable: true),
                    createdon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    modifiedon = table.Column<DateTime>(type: "datetime2", nullable: true),
                    modifiedby = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    record_no = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    appointment_ref = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    appointment_lookup_to = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    doctor_notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    prescribed_treatment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_visit_records", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");

            migrationBuilder.DropTable(
                name: "autonumbers");

            migrationBuilder.DropTable(
                name: "clinic_manager_informations");

            migrationBuilder.DropTable(
                name: "doctor_information_specialization_mtms");

            migrationBuilder.DropTable(
                name: "doctor_informations");

            migrationBuilder.DropTable(
                name: "doctor_leaves");

            migrationBuilder.DropTable(
                name: "doctor_schedules");

            migrationBuilder.DropTable(
                name: "doctor_specializations");

            migrationBuilder.DropTable(
                name: "notifications");

            migrationBuilder.DropTable(
                name: "patient_informations");

            migrationBuilder.DropTable(
                name: "prescriptions");

            migrationBuilder.DropTable(
                name: "receptionist_informations");

            migrationBuilder.DropTable(
                name: "system_users");

            migrationBuilder.DropTable(
                name: "visit_records");
        }
    }
}
