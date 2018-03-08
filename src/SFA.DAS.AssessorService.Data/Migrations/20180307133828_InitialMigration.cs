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
                    EndPointAssessorName = table.Column<string>(maxLength: 100, nullable: false),
                    EndPointAssessorOrganisationId = table.Column<string>(maxLength: 12, nullable: false),
                    EndPointAssessorUkprn = table.Column<int>(nullable: false),
                    PrimaryContact = table.Column<Guid>(maxLength: 30, nullable: true),
                    Status = table.Column<string>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisations", x => x.Id);
                    table.UniqueConstraint("AK_Organisations_EndPointAssessorOrganisationId", x => x.EndPointAssessorOrganisationId);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CertificateData = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 30, nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 30, nullable: true),
                    EndPointAssessorCertificateId = table.Column<int>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(maxLength: 20, nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<string>(maxLength: 30, nullable: true)
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
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    DisplayName = table.Column<string>(maxLength: 120, nullable: false),
                    Email = table.Column<string>(maxLength: 120, nullable: false),
                    EndPointAssessorOrganisationId = table.Column<string>(maxLength: 12, nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    Status = table.Column<string>(maxLength: 10, nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true),
                    Username = table.Column<string>(maxLength: 30, nullable: false)
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
                    Action = table.Column<string>(maxLength: 400, nullable: false),
                    CertificateId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    DeletedAt = table.Column<DateTime>(nullable: true),
                    EventTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<string>(maxLength: 12, nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateLogs", x => x.Id);
                    table.UniqueConstraint("AK_CertificateLogs_EventTime", x => x.EventTime);
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
