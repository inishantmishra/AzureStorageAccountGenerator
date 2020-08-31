using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace AzureStorageAccountGenerator.Migrations
{
    public partial class StorageAccountModelAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DMSServiceInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TenantId = table.Column<Guid>(nullable: false),
                    DatabaseServer = table.Column<string>(nullable: false),
                    DatabaseUser = table.Column<string>(nullable: false),
                    DatabaseName = table.Column<string>(nullable: false),
                    DatabasePassword = table.Column<string>(nullable: false),
                    DatabasePort = table.Column<string>(nullable: false),
                    DMSTenantURL = table.Column<string>(nullable: true),
                    AzStorageConnectionString = table.Column<string>(nullable: true),
                    AzStorageContainer = table.Column<string>(nullable: true),
                    IsDbProvisioned = table.Column<bool>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    IsReplication = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DMSServiceInfo", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DMSServiceInfoId = table.Column<int>(nullable: false),
                    AccountName = table.Column<string>(nullable: true),
                    AccountType = table.Column<string>(nullable: true),
                    AccountId = table.Column<string>(nullable: true),
                    Kind = table.Column<string>(nullable: true),
                    SkuName = table.Column<string>(nullable: true),
                    BlobEndPoints = table.Column<string>(nullable: true),
                    QueueEndPoints = table.Column<string>(nullable: true),
                    FileEndPoints = table.Column<string>(nullable: true),
                    TableEndPoints = table.Column<string>(nullable: true),
                    PrimaryLocation = table.Column<string>(nullable: true),
                    CreationTime = table.Column<DateTime>(nullable: true),
                    LargeFileSharesState = table.Column<string>(nullable: true),
                    AccessTier = table.Column<string>(nullable: true),
                    KeySource = table.Column<string>(nullable: true),
                    IsHnsEnabled = table.Column<bool>(nullable: true),
                    EnableHttpsTrafficOnly = table.Column<bool>(nullable: true),
                    AllowBlobPublicAccess = table.Column<bool>(nullable: true),
                    MinimumTlsVersion = table.Column<string>(nullable: true),
                    TagKey = table.Column<string>(nullable: true),
                    TagValue = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StorageAccounts_DMSServiceInfo_DMSServiceInfoId",
                        column: x => x.DMSServiceInfoId,
                        principalTable: "DMSServiceInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StorageAccounts_DMSServiceInfoId",
                table: "StorageAccounts",
                column: "DMSServiceInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StorageAccounts");

            migrationBuilder.DropTable(
                name: "DMSServiceInfo");
        }
    }
}
