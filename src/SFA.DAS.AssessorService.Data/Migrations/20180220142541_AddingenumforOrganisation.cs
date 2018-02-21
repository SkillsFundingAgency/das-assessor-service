using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Data.Migrations
{
    public partial class AddingenumforOrganisation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisations_Contacts_PrimaryContactId",
                table: "Organisations");

            migrationBuilder.DropIndex(
                name: "IX_Organisations_PrimaryContactId",
                table: "Organisations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Organisations");

            migrationBuilder.AddColumn<int>(
                name: "OrganisationStatus",
                table: "Organisations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationStatus",
                table: "Organisations");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Organisations",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Organisations_PrimaryContactId",
                table: "Organisations",
                column: "PrimaryContactId");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisations_Contacts_PrimaryContactId",
                table: "Organisations",
                column: "PrimaryContactId",
                principalTable: "Contacts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
