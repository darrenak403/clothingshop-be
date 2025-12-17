using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClothingShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPasswordResetHistoryIsExpiredType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Update UsedAt to be nullable
            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "PasswordResetHistories",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            // Step 2: Add a temporary column for the new IsExpired boolean
            migrationBuilder.AddColumn<bool>(
                name: "IsExpiredTemp",
                table: "PasswordResetHistories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            // Step 3: Copy data - if old IsExpired date < now, set new IsExpiredTemp = true
            migrationBuilder.Sql(
                "UPDATE PasswordResetHistories SET IsExpiredTemp = CASE WHEN IsExpired < UTC_TIMESTAMP(6) THEN 1 ELSE 0 END");

            // Step 4: Drop old IsExpired column
            migrationBuilder.DropColumn(
                name: "IsExpired",
                table: "PasswordResetHistories");

            // Step 5: Rename temp column to IsExpired
            migrationBuilder.RenameColumn(
                name: "IsExpiredTemp",
                table: "PasswordResetHistories",
                newName: "IsExpired");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UsedAt",
                table: "PasswordResetHistories",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IsExpired",
                table: "PasswordResetHistories",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }
    }
}
