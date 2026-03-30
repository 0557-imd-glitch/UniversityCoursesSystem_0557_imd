using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityCourseManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAnnotationAndLecturer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Teacher",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "Annotation",
                table: "Courses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Lecturer",
                table: "Courses",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Annotation",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Lecturer",
                table: "Courses");

            migrationBuilder.AddColumn<string>(
                name: "Teacher",
                table: "Courses",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
