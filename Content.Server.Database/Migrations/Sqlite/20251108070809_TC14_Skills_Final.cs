using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Content.Server.Database.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class TC14_Skills_Final : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_passion_profile_id",
                table: "passion");

            migrationBuilder.CreateIndex(
                name: "IX_passion_profile_id_passion_name",
                table: "passion",
                columns: new[] { "profile_id", "passion_name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_passion_profile_id_passion_name",
                table: "passion");

            migrationBuilder.CreateIndex(
                name: "IX_passion_profile_id",
                table: "passion",
                column: "profile_id");
        }
    }
}
