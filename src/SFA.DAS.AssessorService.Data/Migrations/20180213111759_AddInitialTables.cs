using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.Migrations
{
    public partial class AddInitialTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: false),
                    EndPointAssessorName = table.Column<string>(nullable: true),
                    EndPointAssessorOrganisationId = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AchievementDate = table.Column<DateTime>(nullable: false),
                    AchievementOutcome = table.Column<string>(nullable: true),
                    ContactAddLine1 = table.Column<string>(nullable: true),
                    ContactAddLine2 = table.Column<string>(nullable: true),
                    ContactAddLine3 = table.Column<string>(nullable: true),
                    ContactAddLine4 = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    ContactOrganisation = table.Column<string>(nullable: true),
                    ContactPostcode = table.Column<string>(nullable: true),
                    CourseOption = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: false),
                    DeletedBy = table.Column<Guid>(nullable: false),
                    EndPointAssessorCertificateId = table.Column<int>(nullable: false),
                    EndPointAssessorContactId = table.Column<int>(nullable: false),
                    EndPointAssessorOrganisationId = table.Column<string>(nullable: true),
                    LearnerDateofBirth = table.Column<DateTime>(nullable: false),
                    LearnerFamilyName = table.Column<string>(nullable: true),
                    LearnerGivenNames = table.Column<string>(nullable: true),
                    LearnerSex = table.Column<string>(nullable: true),
                    LearningStartDate = table.Column<DateTime>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OverallGrade = table.Column<string>(nullable: true),
                    ProviderUKPRN = table.Column<int>(nullable: false),
                    Registration = table.Column<string>(nullable: true),
                    StandardCode = table.Column<int>(nullable: false),
                    StandardLevel = table.Column<int>(nullable: false),
                    StandardName = table.Column<string>(nullable: true),
                    StandardPublicationDate = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    ULN = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedBY = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Certificates_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ContactEmail = table.Column<string>(nullable: true),
                    ContactName = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: false),
                    EndPointAssessorContactId = table.Column<int>(nullable: false),
                    EndPointAssessorOrganisationId = table.Column<int>(nullable: false),
                    EndPointAssessorUKPRN = table.Column<int>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Organisations_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CertificateLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Action = table.Column<string>(nullable: true),
                    CertificateId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: false),
                    EndPointAssessorCertificateId = table.Column<int>(nullable: false),
                    EventTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateLogs_Certificates_CertificateId",
                        column: x => x.CertificateId,
                        principalTable: "Certificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CertificateLogs_CertificateId",
                table: "CertificateLogs",
                column: "CertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_OrganisationId",
                table: "Certificates",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_OrganisationId",
                table: "Contacts",
                column: "OrganisationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CertificateLogs");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "Organisations");
        }
    }
}
