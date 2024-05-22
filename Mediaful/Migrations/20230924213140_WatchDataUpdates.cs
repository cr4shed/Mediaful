using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Mediaful.Migrations
{
    public partial class WatchDataUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "WatchData");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "WatchData");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "WatchData");

            migrationBuilder.RenameColumn(
                name: "IsFavourite",
                table: "WatchData",
                newName: "IsFavorite");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "WatchData",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "WatchData",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WatchData");

            migrationBuilder.RenameColumn(
                name: "IsFavorite",
                table: "WatchData",
                newName: "IsFavourite");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "WatchData",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "WatchData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "WatchData",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "WatchData",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
