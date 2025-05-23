using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CMS_Api.Migrations
{
    /// <inheritdoc />
    public partial class AddClientModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Clients_ClientId",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Clients_ClientId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ClientId",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Cases_ClientId",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ClientType",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "MobileNumber",
                table: "Clients",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "Gender",
                table: "Clients",
                newName: "RecentCase");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Clients",
                newName: "PassportNumber");

            migrationBuilder.RenameColumn(
                name: "Country",
                table: "Clients",
                newName: "AccountType");

            migrationBuilder.AddColumn<int>(
                name: "ClientId1",
                table: "Documents",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationName",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Clients",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "ClientId1",
                table: "Cases",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                table: "Clients",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ClientId1",
                table: "Documents",
                column: "ClientId1");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_ClientId1",
                table: "Cases",
                column: "ClientId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Clients_ClientId1",
                table: "Cases",
                column: "ClientId1",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Clients_ClientId1",
                table: "Documents",
                column: "ClientId1",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cases_Clients_ClientId1",
                table: "Cases");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Clients_ClientId1",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ClientId1",
                table: "Documents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                table: "Clients");

            migrationBuilder.DropIndex(
                name: "IX_Cases_ClientId1",
                table: "Cases");

            migrationBuilder.DropColumn(
                name: "ClientId1",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "ClientId1",
                table: "Cases");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Clients",
                newName: "MobileNumber");

            migrationBuilder.RenameColumn(
                name: "RecentCase",
                table: "Clients",
                newName: "Gender");

            migrationBuilder.RenameColumn(
                name: "PassportNumber",
                table: "Clients",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "AccountType",
                table: "Clients",
                newName: "Country");

            migrationBuilder.AlterColumn<string>(
                name: "OrganizationName",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Clients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ClientType",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                table: "Clients",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ClientId",
                table: "Documents",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Cases_ClientId",
                table: "Cases",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cases_Clients_ClientId",
                table: "Cases",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Clients_ClientId",
                table: "Documents",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
