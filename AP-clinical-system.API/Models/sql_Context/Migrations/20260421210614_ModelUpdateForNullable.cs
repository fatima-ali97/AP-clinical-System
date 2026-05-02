using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AP_clinical_system.Models.sql_Context.Migrations
{
    /// <inheritdoc />
    public partial class ModelUpdateForNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("0d7c1b0e-4193-4324-8aec-3a4ecb9ee8ac"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("30eef465-6d4d-4475-8727-275cd0921f93"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("4cbe14a0-bbe2-44b2-ad91-c96784254da9"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("66ed2675-98e1-4815-bdaf-16496b6a534c"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("79b76ba8-04cb-45f3-9eba-d40bbcdb0a18"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("7da00435-11e7-4598-8c52-a08e6d77b708"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("82469d6c-8faa-4f85-9b92-6fa9575f61d7"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("89742bd2-f218-4600-a76c-6986a74713d2"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("93c59830-9e39-492c-a974-3015a4bb10c6"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("9dec2755-2918-4950-817a-64d6a8dae85e"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("a5c3c6b3-bb6b-47e9-8515-29e47b492c2f"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("c3867cc9-a345-4868-bd21-95f4b4fe2a45"));

            migrationBuilder.DeleteData(
                table: "autonumbers",
                keyColumn: "id",
                keyValue: new Guid("f0707724-b3d5-4d12-86bd-ad6cb3770b0f"));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                    { new Guid("0d7c1b0e-4193-4324-8aec-3a4ecb9ee8ac"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "notification", "notification_no", null, 1, null, null, "NOTI-{0:D5}" },
                    { new Guid("30eef465-6d4d-4475-8727-275cd0921f93"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "appointment", "appointment_no", null, 1, null, null, "APP-{0:D5}" },
                    { new Guid("4cbe14a0-bbe2-44b2-ad91-c96784254da9"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_specialization", "record_no", null, 1, null, null, "DSP-{0:D5}" },
                    { new Guid("66ed2675-98e1-4815-bdaf-16496b6a534c"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information_specialization_mtm", "record_no", null, 1, null, null, "DIS-{0:D5}" },
                    { new Guid("79b76ba8-04cb-45f3-9eba-d40bbcdb0a18"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "receptionist_information", "record_no", null, 1, null, null, "RINF-{0:D5}" },
                    { new Guid("7da00435-11e7-4598-8c52-a08e6d77b708"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_schedule", "record_no", null, 1, null, null, "DSC-{0:D5}" },
                    { new Guid("82469d6c-8faa-4f85-9b92-6fa9575f61d7"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "clinic_manager_information", "record_no", null, 1, null, null, "CMI-{0:D5}" },
                    { new Guid("89742bd2-f218-4600-a76c-6986a74713d2"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "visit_record", "record_no", null, 1, null, null, "VR-{0:D5}" },
                    { new Guid("93c59830-9e39-492c-a974-3015a4bb10c6"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "prescription", "prescription_no", null, 1, null, null, "PRS-{0:D5}" },
                    { new Guid("9dec2755-2918-4950-817a-64d6a8dae85e"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_leave", "record_no", null, 1, null, null, "DLV-{0:D5}" },
                    { new Guid("a5c3c6b3-bb6b-47e9-8515-29e47b492c2f"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "doctor_information", "record_no", null, 1, null, null, "DINF-{0:D5}" },
                    { new Guid("c3867cc9-a345-4868-bd21-95f4b4fe2a45"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "system_user", "user_no", null, 1, null, null, "USR-{0:D5}" },
                    { new Guid("f0707724-b3d5-4d12-86bd-ad6cb3770b0f"), null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "patient_information", "record_no", null, 1, null, null, "PINF-{0:D5}" }
                });
        }
    }
}
