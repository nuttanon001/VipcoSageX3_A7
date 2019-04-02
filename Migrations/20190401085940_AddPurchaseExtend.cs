using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VipcoSageX3.Migrations
{
    public partial class AddPurchaseExtend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchaseOrderHeader",
                columns: table => new
                {
                    PurchaseOrderHeaderId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Creator = table.Column<string>(maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Modifyer = table.Column<string>(maxLength: 50, nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true),
                    PrReceivedDate = table.Column<DateTime>(nullable: true),
                    PrReceivedTime = table.Column<string>(maxLength: 10, nullable: true),
                    Remark = table.Column<string>(maxLength: 350, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderHeader", x => x.PurchaseOrderHeaderId);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseExtend",
                columns: table => new
                {
                    PurchaseExtendId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Creator = table.Column<string>(maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Modifyer = table.Column<string>(maxLength: 50, nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true),
                    PrSageHeaderId = table.Column<int>(nullable: true),
                    PRNumber = table.Column<string>(maxLength: 20, nullable: true),
                    PrReceivedDate = table.Column<DateTime>(nullable: true),
                    PrReceivedTime = table.Column<string>(maxLength: 10, nullable: true),
                    Remark = table.Column<string>(maxLength: 350, nullable: true),
                    PurchaseOrderHeaderId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseExtend", x => x.PurchaseExtendId);
                    table.ForeignKey(
                        name: "FK_PurchaseExtend_PurchaseOrderHeader_PurchaseOrderHeaderId",
                        column: x => x.PurchaseOrderHeaderId,
                        principalTable: "PurchaseOrderHeader",
                        principalColumn: "PurchaseOrderHeaderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseLineExtend",
                columns: table => new
                {
                    PurchaseLineExtendId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Creator = table.Column<string>(maxLength: 50, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: true),
                    Modifyer = table.Column<string>(maxLength: 50, nullable: true),
                    ModifyDate = table.Column<DateTime>(nullable: true),
                    PrSageLineId = table.Column<int>(nullable: true),
                    PrNumber = table.Column<string>(maxLength: 20, nullable: true),
                    PrLine = table.Column<int>(nullable: false),
                    ItemCode = table.Column<string>(maxLength: 50, nullable: true),
                    ItemName = table.Column<string>(maxLength: 500, nullable: true),
                    Remark = table.Column<string>(maxLength: 350, nullable: true),
                    Quantity = table.Column<double>(nullable: true),
                    PurchaseExtendId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseLineExtend", x => x.PurchaseLineExtendId);
                    table.ForeignKey(
                        name: "FK_PurchaseLineExtend_PurchaseExtend_PurchaseExtendId",
                        column: x => x.PurchaseExtendId,
                        principalTable: "PurchaseExtend",
                        principalColumn: "PurchaseExtendId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseExtend_PurchaseOrderHeaderId",
                table: "PurchaseExtend",
                column: "PurchaseOrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseExtend_PRNumber_PrSageHeaderId",
                table: "PurchaseExtend",
                columns: new[] { "PRNumber", "PrSageHeaderId" },
                unique: true,
                filter: "[PRNumber] IS NOT NULL AND [PrSageHeaderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseLineExtend_PurchaseExtendId",
                table: "PurchaseLineExtend",
                column: "PurchaseExtendId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchaseLineExtend");

            migrationBuilder.DropTable(
                name: "PurchaseExtend");

            migrationBuilder.DropTable(
                name: "PurchaseOrderHeader");
        }
    }
}
