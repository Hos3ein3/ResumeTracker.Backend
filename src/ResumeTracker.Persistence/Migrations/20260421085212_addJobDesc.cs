using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addJobDesc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JobDescription",
                table: "Resumes",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobDescription",
                table: "Resumes");
        }
    }
}
