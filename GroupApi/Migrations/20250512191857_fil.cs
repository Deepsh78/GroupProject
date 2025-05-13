using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GroupApi.Migrations
{
    /// <inheritdoc />
    public partial class fil : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileMetadata_Member_MemberId",
                table: "FileMetadata");

            migrationBuilder.DropIndex(
                name: "IX_FileMetadata_MemberId",
                table: "FileMetadata");

            migrationBuilder.DropColumn(
                name: "MemberId",
                table: "FileMetadata");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MemberId",
                table: "FileMetadata",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FileMetadata_MemberId",
                table: "FileMetadata",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_FileMetadata_Member_MemberId",
                table: "FileMetadata",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "MemberId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
