using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VipcoSageX3.Migrations
{
    public partial class inti : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptExtend",
                columns: table => new
                {
                    ReceiptExtendId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Creator = table.Column<string>(maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Modifyer = table.Column<string>(maxLength: 50, nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true),
                    StartRange = table.Column<int>(nullable: true),
                    EndRange = table.Column<int>(nullable: true),
                    GetDate = table.Column<DateTime>(nullable: true),
                    GetTime = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptExtend", x => x.ReceiptExtendId);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatusMaster",
                columns: table => new
                {
                    TaskStatusMasterId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Creator = table.Column<string>(maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Modifyer = table.Column<string>(maxLength: 50, nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true),
                    WorkGroupCode = table.Column<string>(maxLength: 50, nullable: true),
                    WorkGroupName = table.Column<string>(maxLength: 200, nullable: true),
                    Remark = table.Column<string>(maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatusMaster", x => x.TaskStatusMasterId);
                });

            migrationBuilder.CreateTable(
                name: "TaskStatusDetail",
                columns: table => new
                {
                    TaskStatusDetailId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Creator = table.Column<string>(maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Modifyer = table.Column<string>(maxLength: 50, nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true),
                    EmployeeCode = table.Column<string>(maxLength: 20, nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: true),
                    Email = table.Column<string>(maxLength: 250, nullable: true),
                    Remark = table.Column<string>(maxLength: 200, nullable: true),
                    TaskStatusMasterId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskStatusDetail", x => x.TaskStatusDetailId);
                    table.ForeignKey(
                        name: "FK_TaskStatusDetail_TaskStatusMaster_TaskStatusMasterId",
                        column: x => x.TaskStatusMasterId,
                        principalTable: "TaskStatusMaster",
                        principalColumn: "TaskStatusMasterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusDetail_TaskStatusMasterId",
                table: "TaskStatusDetail",
                column: "TaskStatusMasterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptExtend");

            migrationBuilder.DropTable(
                name: "TaskStatusDetail");

            migrationBuilder.DropTable(
                name: "TaskStatusMaster");
        }
    }
}
