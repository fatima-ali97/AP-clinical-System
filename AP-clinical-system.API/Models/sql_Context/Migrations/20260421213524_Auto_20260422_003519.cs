using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AP_clinical_system.Models.sql_Context.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20260422_003519 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"));

            migrationBuilder.DeleteData(
                table: "system_users",
                keyColumn: "id",
                keyValue: new Guid("b9c9f28d-19df-4c31-92f7-5ebd7e5dc8d1"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "autonumbers",
                columns: new[] { "id", "createdby", "createdon", "entity_name", "field_name", "inactive", "last_number", "modifiedby", "modifiedon", "pattern" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "appointment", "appointment_no", null, 1, null, null, "APP-{0:D5}" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "clinic_manager_information", "record_no", null, 1, null, null, "CMI-{0:D5}" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information", "record_no", null, 1, null, null, "DINF-{0:D5}" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information_specialization_mtm", "record_no", null, 1, null, null, "DIS-{0:D5}" },
                    { new Guid("55555555-5555-5555-5555-555555555555"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_leave", "record_no", null, 1, null, null, "DLV-{0:D5}" },
                    { new Guid("66666666-6666-6666-6666-666666666666"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_schedule", "record_no", null, 1, null, null, "DSC-{0:D5}" },
                    { new Guid("77777777-7777-7777-7777-777777777777"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_specialization", "record_no", null, 1, null, null, "DSP-{0:D5}" },
                    { new Guid("88888888-8888-8888-8888-888888888888"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "notification", "notification_no", null, 1, null, null, "NOTI-{0:D5}" },
                    { new Guid("99999999-9999-9999-9999-999999999999"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "patient_information", "record_no", null, 1, null, null, "PINF-{0:D5}" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "prescription", "prescription_no", null, 1, null, null, "PRS-{0:D5}" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "receptionist_information", "record_no", null, 1, null, null, "RINF-{0:D5}" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system_user", "user_no", null, 1, null, null, "USR-{0:D5}" },
                    { new Guid("dddddddd-dddd-dddd-dddd-dddddddddddd"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "visit_record", "record_no", null, 1, null, null, "VR-{0:D5}" }
                });

            migrationBuilder.InsertData(
                table: "system_users",
                columns: new[] { "id", "cpr", "createdby", "createdon", "email", "firt_name", "hashed_password", "inactive", "last_name", "modifiedby", "modifiedon", "phone_number", "user_no", "user_role" },
                values: new object[] { new Guid("b9c9f28d-19df-4c31-92f7-5ebd7e5dc8d1"), "040400069", new Guid("b9c9f28d-19df-4c31-92f7-5ebd7e5dc8d1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "abbasfadhel499@gmail.com", "Abbas", "123", false, "Aljamri", new Guid("b9c9f28d-19df-4c31-92f7-5ebd7e5dc8d1"), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "34309097", "USR-00001", 1000 });
        }
    }
}
