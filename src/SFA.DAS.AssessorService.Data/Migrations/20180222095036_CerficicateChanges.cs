using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.Migrations
{
    public partial class CerficicateChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AchievementDate",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "AchievementOutcome",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ContactAddLine1",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ContactAddLine2",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ContactAddLine3",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ContactAddLine4",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ContactOrganisation",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ContactPostCode",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "CourseOption",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "EndPointAssessorCertificateId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "EndPointAssessorContactId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "EndPointAssessorOrganisationId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LearnerDateofBirth",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LearnerFamilyName",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LearnerGivenNames",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LearnerSex",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "LearningStartDate",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "OverallGrade",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ProviderUKPRN",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "Registration",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "StandardCode",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "StandardLevel",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "StandardPublicationDate",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "ULN",
                table: "Certificates");

            migrationBuilder.RenameColumn(
                name: "UpdatedBY",
                table: "Certificates",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "StandardName",
                table: "Certificates",
                newName: "CertificateData");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Certificates",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "CertificateLogs",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Certificates",
                newName: "UpdatedBY");

            migrationBuilder.RenameColumn(
                name: "CertificateData",
                table: "Certificates",
                newName: "StandardName");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<DateTime>(
                name: "AchievementDate",
                table: "Certificates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AchievementOutcome",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddLine1",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddLine2",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddLine3",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactAddLine4",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactOrganisation",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactPostCode",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CourseOption",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EndPointAssessorCertificateId",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EndPointAssessorContactId",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EndPointAssessorOrganisationId",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LearnerDateofBirth",
                table: "Certificates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LearnerFamilyName",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearnerGivenNames",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LearnerSex",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LearningStartDate",
                table: "Certificates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OverallGrade",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderUKPRN",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Registration",
                table: "Certificates",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StandardCode",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StandardLevel",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StandardPublicationDate",
                table: "Certificates",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ULN",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CertificateLogs",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
