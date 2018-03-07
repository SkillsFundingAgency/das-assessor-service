using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.Migrations
{
    public partial class CertificateChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndPointAssessorCertificateId",
                table: "CertificateLogs");

            migrationBuilder.AddColumn<int>(
                name: "EndPointAssessorCertificateId",
                table: "Certificates",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndPointAssessorCertificateId",
                table: "Certificates");

            migrationBuilder.AddColumn<int>(
                name: "EndPointAssessorCertificateId",
                table: "CertificateLogs",
                nullable: false,
                defaultValue: 0);
        }
    }
}
