using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Api.Migrations
{
    public partial class changedNeuralNetmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetType",
                table: "NeuralNets");

            migrationBuilder.AddColumn<int>(
                name: "NetFuncType",
                table: "NeuralNets",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "NeuralNets",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetFuncType",
                table: "NeuralNets");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "NeuralNets");

            migrationBuilder.AddColumn<int>(
                name: "NetType",
                table: "NeuralNets",
                nullable: false,
                defaultValue: 0);
        }
    }
}
