using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JobScheduler.Migrations
{
    public partial class jobGroup_removed_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExecutionResult",
                table: "JobGroup");

            migrationBuilder.DropColumn(
                name: "LastExecutionDate",
                table: "JobGroup");

            migrationBuilder.DropColumn(
                name: "OutputResult",
                table: "JobGroup");

            migrationBuilder.DropColumn(
                name: "Pid",
                table: "JobGroup");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExecutionResult",
                table: "JobGroup",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastExecutionDate",
                table: "JobGroup",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OutputResult",
                table: "JobGroup",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pid",
                table: "JobGroup",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
