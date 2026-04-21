using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AP_clinical_system.Models.sql_Context.Migrations
{
    /// <inheritdoc />
    public partial class Auto_20260422_000703 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("0635f853-0267-4160-bd20-058936353d4a"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("286bb02d-a01c-4895-a77d-15aebccc7af2"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("32bec5fd-5b47-4c55-a67f-dfe1abef7a3c"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("34186f8c-36bc-4846-8532-356d87d6ff5e"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("4ea131a1-69a8-4b16-b378-f3f8ab68aab7"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("4f22e6eb-35b7-46c3-ba05-e5ad8c079142"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("53ff12ad-3d2e-49c6-8043-ec5ba79940fd"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("746d4975-4013-4ff3-abde-dff8f4bbe554"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("8bc1ec1d-60e6-4b10-a083-1d7993ec2602"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("a4fb3154-6358-444f-a8fe-3e4d9e829ec2"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("b91bf242-b386-4df3-8cdf-9843b5874699"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("c2ca50d2-5d07-44bd-b276-dbeaa3be35d2"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("c463e8b8-3d2d-4584-9807-a3bf41aa5678"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { new Guid("0635f853-0267-4160-bd20-058936353d4a"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "visit_record", "record_no", null, 1, null, null, "VR-{0:D5}" },
                    { new Guid("286bb02d-a01c-4895-a77d-15aebccc7af2"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "patient_information", "record_no", null, 1, null, null, "PINF-{0:D5}" },
                    { new Guid("32bec5fd-5b47-4c55-a67f-dfe1abef7a3c"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_specialization", "record_no", null, 1, null, null, "DSP-{0:D5}" },
                    { new Guid("34186f8c-36bc-4846-8532-356d87d6ff5e"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_leave", "record_no", null, 1, null, null, "DLV-{0:D5}" },
                    { new Guid("4ea131a1-69a8-4b16-b378-f3f8ab68aab7"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "clinic_manager_information", "record_no", null, 1, null, null, "CMI-{0:D5}" },
                    { new Guid("4f22e6eb-35b7-46c3-ba05-e5ad8c079142"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information", "record_no", null, 1, null, null, "DINF-{0:D5}" },
                    { new Guid("53ff12ad-3d2e-49c6-8043-ec5ba79940fd"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_schedule", "record_no", null, 1, null, null, "DSC-{0:D5}" },
                    { new Guid("746d4975-4013-4ff3-abde-dff8f4bbe554"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "receptionist_information", "record_no", null, 1, null, null, "RINF-{0:D5}" },
                    { new Guid("8bc1ec1d-60e6-4b10-a083-1d7993ec2602"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "prescription", "prescription_no", null, 1, null, null, "PRS-{0:D5}" },
                    { new Guid("a4fb3154-6358-444f-a8fe-3e4d9e829ec2"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system_user", "user_no", null, 1, null, null, "USR-{0:D5}" },
                    { new Guid("b91bf242-b386-4df3-8cdf-9843b5874699"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information_specialization_mtm", "record_no", null, 1, null, null, "DIS-{0:D5}" },
                    { new Guid("c2ca50d2-5d07-44bd-b276-dbeaa3be35d2"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "appointment", "appointment_no", null, 1, null, null, "APP-{0:D5}" },
                    { new Guid("c463e8b8-3d2d-4584-9807-a3bf41aa5678"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "notification", "notification_no", null, 1, null, null, "NOTI-{0:D5}" }
                });
        }
    }
}
