using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageSpark.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddBlogPostMediaRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CoverMediaFileId",
                table: "BlogPosts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "BlogPostMediaFiles",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "BlogPostMediaFiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_CoverMediaFileId",
                table: "BlogPosts",
                column: "CoverMediaFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_MediaFiles_CoverMediaFileId",
                table: "BlogPosts",
                column: "CoverMediaFileId",
                principalTable: "MediaFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_MediaFiles_CoverMediaFileId",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_CoverMediaFileId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "CoverMediaFileId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "BlogPostMediaFiles");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "BlogPostMediaFiles");
        }
    }
}
