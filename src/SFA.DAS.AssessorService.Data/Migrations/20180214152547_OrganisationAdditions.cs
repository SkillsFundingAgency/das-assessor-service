using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.Migrations
{
    public partial class OrganisationAdditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ContactPostcode",
                table: "Certificates",
                newName: "ContactPostCode");

            migrationBuilder.AddColumn<int>(
                name: "EndPointAssessorUKPRN",
                table: "Organisations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PrimaryContactId",
                table: "Organisations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndPointAssessorUKPRN",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "PrimaryContactId",
                table: "Organisations");

            migrationBuilder.RenameColumn(
                name: "ContactPostCode",
                table: "Certificates",
                newName: "ContactPostcode");
        }
    }
}
