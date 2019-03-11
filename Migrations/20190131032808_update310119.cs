using Microsoft.EntityFrameworkCore.Migrations;

namespace VipcoSageX3.Migrations
{
    public partial class update310119 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusMaster_WorkGroupCode",
                table: "TaskStatusMaster",
                column: "WorkGroupCode",
                unique: true,
                filter: "[WorkGroupCode] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TaskStatusMaster_WorkGroupCode",
                table: "TaskStatusMaster");
        }
    }
}
