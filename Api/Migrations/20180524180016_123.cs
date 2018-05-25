using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Api.Migrations
{
    public partial class _123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "NeuralNets");

            migrationBuilder.AddColumn<string>(
                name: "StorageKey",
                table: "NeuralNets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StorageKey",
                table: "NeuralNets");

            migrationBuilder.AddColumn<byte[]>(
                name: "Body",
                table: "NeuralNets",
                nullable: true);
        }
    }
}
