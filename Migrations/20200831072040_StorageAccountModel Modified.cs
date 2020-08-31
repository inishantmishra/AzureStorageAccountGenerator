using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureStorageAccountGenerator.Migrations
{
    public partial class StorageAccountModelModified : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeyAccessName",
                table: "StorageAccounts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KeyAccessValue",
                table: "StorageAccounts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeyAccessName",
                table: "StorageAccounts");

            migrationBuilder.DropColumn(
                name: "KeyAccessValue",
                table: "StorageAccounts");
        }
    }
}
