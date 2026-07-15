using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediConnect.Api.Migrations
{
    /// <inheritdoc />
    public partial class ReplaceWeightWithSpO2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Weight",
                table: "Vitals",
                newName: "SpO2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SpO2",
                table: "Vitals",
                newName: "Weight");
        }
    }
}
