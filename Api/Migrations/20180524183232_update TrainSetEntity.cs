using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Api.Migrations
{
    public partial class updateTrainSetEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "TrainSets");

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "TrainSets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "TrainSets");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "TrainSets",
                nullable: true);
        }
    }
}
