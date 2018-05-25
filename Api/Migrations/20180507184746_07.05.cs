using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Api.Migrations
{
    public partial class _0705 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastCheckTime",
                table: "TrainSets",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ScheduleMessage",
                table: "TrainSets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleStatus",
                table: "TrainSets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "TrainSets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCheckTime",
                table: "TrainSets");

            migrationBuilder.DropColumn(
                name: "ScheduleMessage",
                table: "TrainSets");

            migrationBuilder.DropColumn(
                name: "ScheduleStatus",
                table: "TrainSets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TrainSets");
        }
    }
}
