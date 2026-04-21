using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeTracker.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ResumeStatusADd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApplyDate",
                table: "Resumes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "ResumeStatus",
                table: "Resumes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyDate",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "ResumeStatus",
                table: "Resumes");
        }
    }
}
