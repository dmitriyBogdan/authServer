using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuthServerDemo.Data.Migrations.IdentityServer.AuthServerDemo
{
    public partial class ThirdPartyProviderSupport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProviderName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderSubjectId",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProviderName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProviderSubjectId",
                table: "AspNetUsers");
        }
    }
}
