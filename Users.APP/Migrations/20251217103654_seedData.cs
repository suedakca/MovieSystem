using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Users.APP.Migrations
{
    /// <inheritdoc />
    public partial class seedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "UserMovies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserMovies",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MovieName",
                table: "UserMovies",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "Guid", "Title" },
                values: new object[,]
                {
                    { 1, null, "Child" },
                    { 2, null, "Adult" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Guid", "Name" },
                values: new object[,]
                {
                    { 1, null, "Admin" },
                    { 2, null, "Customer" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "UserMovies");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserMovies");

            migrationBuilder.DropColumn(
                name: "MovieName",
                table: "UserMovies");
        }
    }
}
