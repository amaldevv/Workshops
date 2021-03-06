﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcSampleDemo.Migrations
{
    public partial class AddedAge : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "Employee",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Employee");
        }
    }
}
