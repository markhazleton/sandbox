using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GarageSpark.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddBlogPostFrontmatterSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FrontmatterJson",
                table: "BlogPosts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "BlogPosts",
                type: "TEXT",
                maxLength: 160,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MetaKeywords",
                table: "BlogPosts",
                type: "TEXT",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MetaTitle",
                table: "BlogPosts",
                type: "TEXT",
                maxLength: 60,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProcessedHtml",
                table: "BlogPosts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ReadingTimeMinutes",
                table: "BlogPosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WordCount",
                table: "BlogPosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrontmatterJson",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "MetaKeywords",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "MetaTitle",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ProcessedHtml",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ReadingTimeMinutes",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "WordCount",
                table: "BlogPosts");
        }
    }
}
