using Microsoft.EntityFrameworkCore.Migrations;

namespace JobScheduler.Migrations
{
    public partial class job_add_argomenti : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IndirizzoIp",
                table: "Node",
                newName: "IndirizzoIP");

            migrationBuilder.AddColumn<string>(
                name: "Argomenti",
                table: "Job",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Argomenti",
                table: "Job");

            migrationBuilder.RenameColumn(
                name: "IndirizzoIP",
                table: "Node",
                newName: "IndirizzoIp");
        }
    }
}
