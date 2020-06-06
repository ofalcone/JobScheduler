using Microsoft.EntityFrameworkCore.Migrations;

namespace JobScheduler.Migrations
{
    public partial class JobGroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobNode");

            migrationBuilder.CreateTable(
                name: "JobGroup",
                columns: table => new
                {
                    JobId = table.Column<int>(nullable: false),
                    GroupId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobGroup", x => new { x.JobId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_JobGroup_Group_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobGroup_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobGroup_GroupId",
                table: "JobGroup",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobGroup");

            migrationBuilder.CreateTable(
                name: "JobNode",
                columns: table => new
                {
                    JobId = table.Column<int>(type: "int", nullable: false),
                    NodeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobNode", x => new { x.JobId, x.NodeId });
                    table.ForeignKey(
                        name: "FK_JobNode_Job_JobId",
                        column: x => x.JobId,
                        principalTable: "Job",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobNode_Node_NodeId",
                        column: x => x.NodeId,
                        principalTable: "Node",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobNode_NodeId",
                table: "JobNode",
                column: "NodeId");
        }
    }
}
