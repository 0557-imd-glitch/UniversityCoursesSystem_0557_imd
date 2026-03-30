using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniversityCourseManager.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Specialties_SpecialtyId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_SpecialtyId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "Courses");

            migrationBuilder.CreateTable(
                name: "CourseSpecialties",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecialtyId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseSpecialties", x => new { x.CourseId, x.SpecialtyId });
                    table.ForeignKey(
                        name: "FK_CourseSpecialties_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseSpecialties_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseSpecialties_SpecialtyId",
                table: "CourseSpecialties",
                column: "SpecialtyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseSpecialties");

            migrationBuilder.AddColumn<int>(
                name: "SpecialtyId",
                table: "Courses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_SpecialtyId",
                table: "Courses",
                column: "SpecialtyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Specialties_SpecialtyId",
                table: "Courses",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
