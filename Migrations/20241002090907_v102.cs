﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP.Migrations
{
    /// <inheritdoc />
    public partial class v102 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateRefuseForApproved",
                table: "KPIDetail");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "KPIDetail");

            migrationBuilder.DropColumn(
                name: "IsCompleteApproved",
                table: "KPIDetail");

            migrationBuilder.DropColumn(
                name: "IsRefuseForApproved",
                table: "KPIDetail");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRefuseForApproved",
                table: "KPIs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleteApproved",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefuseForApproved",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "WillApproved",
                table: "KPIs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateRefuseForApproved",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "IsCompleteApproved",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "IsRefuseForApproved",
                table: "KPIs");

            migrationBuilder.DropColumn(
                name: "WillApproved",
                table: "KPIs");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRefuseForApproved",
                table: "KPIDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "KPIDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleteApproved",
                table: "KPIDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRefuseForApproved",
                table: "KPIDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
