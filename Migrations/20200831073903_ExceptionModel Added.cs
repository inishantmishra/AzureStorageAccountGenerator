using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AzureStorageAccountGenerator.Migrations
{
    public partial class ExceptionModelAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExceptionLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DMSServiceInfoId = table.Column<int>(nullable: false),
                    ExceptionMessage = table.Column<string>(nullable: true),
                    StackTrace = table.Column<string>(nullable: true),
                    InnerException = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExceptionLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExceptionLogs_DMSServiceInfo_DMSServiceInfoId",
                        column: x => x.DMSServiceInfoId,
                        principalTable: "DMSServiceInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExceptionLogs_DMSServiceInfoId",
                table: "ExceptionLogs",
                column: "DMSServiceInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExceptionLogs");
        }
    }
}
