using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organisations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    EndPointAssessorName = table.Column<string>(nullable: true),
                    EndPointAssessorOrganisationId = table.Column<string>(nullable: true),
                    EndPointAssessorUkprn = table.Column<int>(nullable: false),
                    OrganisationStatus = table.Column<int>(nullable: false),
                    PrimaryContactId = table.Column<Guid>(nullable: true),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
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
                    CertificateData = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<Guid>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<Guid>(nullable: false)
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
                    ContactStatus = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EndPointAssessorOrganisationId = table.Column<string>(nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Username = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.UniqueConstraint("AK_Contacts_Username", x => x.Username);
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
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    EndPointAssessorCertificateId = table.Column<int>(nullable: false),
                    EventTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
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
