using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AP_clinical_system.Models.sql_Context.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20260422_001209 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("051aad61-f034-491f-ae19-cdf968b8bd0d"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("0b01d7ec-f488-4ab7-8b0d-8ff8955b7fba"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("0fbb23e9-3541-48c5-a17c-d5acc2edd91d"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("1dcf32bc-1ef7-49d0-8e00-857e9db799e9"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("4a6395ca-bdf7-4155-b6ca-843423ae5687"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("5b7ea96f-7e25-41ac-8873-77fb6d3c1b4c"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("7143b283-51c6-4199-83bf-195444dcafef"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("b293bac4-3997-4872-8a2e-ea18c9c01e94"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("c2c16474-bda4-4526-b8e6-4c441dd1b940"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("ca0aefb2-0e68-49c5-86ac-dd7276004c7b"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("e5a41fff-c4df-46a7-9c07-11f1f4b520bb"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("f370d4bc-36c4-4611-9d78-5ed538759cfb"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("f96801e9-ac6a-4d93-9dd7-867f42432256"));

            migrationBuilder.InsertData(
                table: "autonumbers",
                columns: new[] { "id", "createdby", "createdon", "entity_name", "field_name", "inactive", "last_number", "modifiedby", "modifiedon", "pattern" },
                values: new object[,]
                {
                    { new Guid("4953e1b0-2b11-4fd8-b840-3e575c7cbc3f"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "appointment", "appointment_no", null, 1, null, null, "APP-{0:D5}" },
                    { new Guid("582e816b-dbe0-4927-8b39-4377122728fd"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "patient_information", "record_no", null, 1, null, null, "PINF-{0:D5}" },
                    { new Guid("7b1b4bb9-5099-49ab-adb9-edd70e446704"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information", "record_no", null, 1, null, null, "DINF-{0:D5}" },
                    { new Guid("8a71d6cb-d1a6-464d-90c0-212e28a7cf41"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information_specialization_mtm", "record_no", null, 1, null, null, "DIS-{0:D5}" },
                    { new Guid("8daeadf8-d42a-482f-b7cb-28fbfc7308f8"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_leave", "record_no", null, 1, null, null, "DLV-{0:D5}" },
                    { new Guid("a4c99fc0-8b0a-4bb0-b353-21b11e48bbc1"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "clinic_manager_information", "record_no", null, 1, null, null, "CMI-{0:D5}" },
                    { new Guid("a5185f4f-bdb6-4ee6-a411-0ca50d2700f6"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "notification", "notification_no", null, 1, null, null, "NOTI-{0:D5}" },
                    { new Guid("ad994389-1a9d-46af-8c28-ac8b70001e84"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_specialization", "record_no", null, 1, null, null, "DSP-{0:D5}" },
                    { new Guid("ba282ea3-2995-4148-ba2b-71d84079c2bf"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "visit_record", "record_no", null, 1, null, null, "VR-{0:D5}" },
                    { new Guid("c3cb31c7-ba47-4bec-ba7a-7ad58fa7f1ce"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "prescription", "prescription_no", null, 1, null, null, "PRS-{0:D5}" },
                    { new Guid("e5887b6e-a0d4-4e2a-ad1a-11bc93f6ee1b"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "receptionist_information", "record_no", null, 1, null, null, "RINF-{0:D5}" },
                    { new Guid("ed1d8c36-a354-4d05-9d2d-cff65844117e"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_schedule", "record_no", null, 1, null, null, "DSC-{0:D5}" },
                    { new Guid("ef4498b9-8a80-4553-86a4-cedd60d6ac21"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system_user", "user_no", null, 1, null, null, "USR-{0:D5}" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("4953e1b0-2b11-4fd8-b840-3e575c7cbc3f"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("582e816b-dbe0-4927-8b39-4377122728fd"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("7b1b4bb9-5099-49ab-adb9-edd70e446704"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("8a71d6cb-d1a6-464d-90c0-212e28a7cf41"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("8daeadf8-d42a-482f-b7cb-28fbfc7308f8"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("a4c99fc0-8b0a-4bb0-b353-21b11e48bbc1"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("a5185f4f-bdb6-4ee6-a411-0ca50d2700f6"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("ad994389-1a9d-46af-8c28-ac8b70001e84"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("ba282ea3-2995-4148-ba2b-71d84079c2bf"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("c3cb31c7-ba47-4bec-ba7a-7ad58fa7f1ce"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("e5887b6e-a0d4-4e2a-ad1a-11bc93f6ee1b"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("ed1d8c36-a354-4d05-9d2d-cff65844117e"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("ef4498b9-8a80-4553-86a4-cedd60d6ac21"));

            migrationBuilder.InsertData(
                table: "autonumbers",
                columns: new[] { "id", "createdby", "createdon", "entity_name", "field_name", "inactive", "last_number", "modifiedby", "modifiedon", "pattern" },
                values: new object[,]
                {
                    { new Guid("051aad61-f034-491f-ae19-cdf968b8bd0d"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_schedule", "record_no", null, 1, null, null, "DSC-{0:D5}" },
                    { new Guid("0b01d7ec-f488-4ab7-8b0d-8ff8955b7fba"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_specialization", "record_no", null, 1, null, null, "DSP-{0:D5}" },
                    { new Guid("0fbb23e9-3541-48c5-a17c-d5acc2edd91d"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "visit_record", "record_no", null, 1, null, null, "VR-{0:D5}" },
                    { new Guid("1dcf32bc-1ef7-49d0-8e00-857e9db799e9"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "receptionist_information", "record_no", null, 1, null, null, "RINF-{0:D5}" },
                    { new Guid("4a6395ca-bdf7-4155-b6ca-843423ae5687"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "patient_information", "record_no", null, 1, null, null, "PINF-{0:D5}" },
                    { new Guid("5b7ea96f-7e25-41ac-8873-77fb6d3c1b4c"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information", "record_no", null, 1, null, null, "DINF-{0:D5}" },
                    { new Guid("7143b283-51c6-4199-83bf-195444dcafef"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "appointment", "appointment_no", null, 1, null, null, "APP-{0:D5}" },
                    { new Guid("b293bac4-3997-4872-8a2e-ea18c9c01e94"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_leave", "record_no", null, 1, null, null, "DLV-{0:D5}" },
                    { new Guid("c2c16474-bda4-4526-b8e6-4c441dd1b940"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information_specialization_mtm", "record_no", null, 1, null, null, "DIS-{0:D5}" },
                    { new Guid("ca0aefb2-0e68-49c5-86ac-dd7276004c7b"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system_user", "user_no", null, 1, null, null, "USR-{0:D5}" },
                    { new Guid("e5a41fff-c4df-46a7-9c07-11f1f4b520bb"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "prescription", "prescription_no", null, 1, null, null, "PRS-{0:D5}" },
                    { new Guid("f370d4bc-36c4-4611-9d78-5ed538759cfb"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "clinic_manager_information", "record_no", null, 1, null, null, "CMI-{0:D5}" },
                    { new Guid("f96801e9-ac6a-4d93-9dd7-867f42432256"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "notification", "notification_no", null, 1, null, null, "NOTI-{0:D5}" }
                });
        }
    }
}
