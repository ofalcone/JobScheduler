using Microsoft.EntityFrameworkCore.Migrations;

namespace JobScheduler.Migrations
{
    public partial class NodeLaunchResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LaunchResult",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaunchResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NodeLaunchResult",
                columns: table => new
                {
                    NodeId = table.Column<int>(nullable: false),
                    LaunchResultId = table.Column<int>(nullable: false),
                    Pid = table.Column<int>(nullable: false),
                    ExitCode = table.Column<int>(nullable: false),
                    StandardOutput = table.Column<string>(nullable: true),
                    JobId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NodeLaunchResult", x => new { x.NodeId, x.LaunchResultId });
                    table.ForeignKey(
                        name: "FK_NodeLaunchResult_LaunchResult_LaunchResultId",
                        column: x => x.LaunchResultId,
                        principalTable: "LaunchResult",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NodeLaunchResult_Node_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Node",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NodeLaunchResult_LaunchResultId",
                table: "NodeLaunchResult",
                column: "LaunchResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NodeLaunchResult");

            migrationBuilder.DropTable(
                name: "LaunchResult");
        }
    }
}
