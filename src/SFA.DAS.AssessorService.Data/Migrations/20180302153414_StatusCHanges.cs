using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.Migrations
{
    public partial class StatusCHanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationStatus",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "ContactStatus",
                table: "Contacts");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Organisations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Contacts",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Certificates",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "CertificateLogs",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Contacts");

            migrationBuilder.AddColumn<int>(
                name: "OrganisationStatus",
                table: "Organisations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContactStatus",
                table: "Contacts",
                nullable: false,
                defaultValue: 0);

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
    }
}
