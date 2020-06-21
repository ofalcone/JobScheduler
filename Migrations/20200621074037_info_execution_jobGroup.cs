using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JobScheduler.Migrations
{
    public partial class info_execution_jobGroup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IndirizzoIp",
                table: "Node",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExecutionResult",
                table: "JobGroup",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastExecutionDate",
                table: "JobGroup",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "OutputResult",
                table: "JobGroup",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Pid",
                table: "JobGroup",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IndirizzoIp",
                table: "Node");

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
    }
}
