using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Content.Server.Database.Migrations.Postgres
{
    /// <inheritdoc />
    public partial class _13_09_26 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_MoffPreference_TempId",
                table: "MoffPreference");

            migrationBuilder.RenameTable(
                name: "MoffPreference",
                newName: "moff_preference");

            migrationBuilder.RenameColumn(
                name: "PreferenceId",
                table: "moff_preference",
                newName: "preference_id");

            migrationBuilder.RenameColumn(
                name: "TempId",
                table: "moff_preference",
                newName: "moff_preference_id");

            migrationBuilder.AlterColumn<int>(
                name: "preference_id",
                table: "moff_preference",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "moff_preference_id",
                table: "moff_preference",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_moff_preference",
                table: "moff_preference",
                column: "moff_preference_id");

            migrationBuilder.CreateIndex(
                name: "IX_moff_preference_preference_id",
                table: "moff_preference",
                column: "preference_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_moff_preference",
                table: "moff_preference");

            migrationBuilder.DropIndex(
                name: "IX_moff_preference_preference_id",
                table: "moff_preference");

            migrationBuilder.RenameTable(
                name: "moff_preference",
                newName: "MoffPreference");

            migrationBuilder.RenameColumn(
                name: "preference_id",
                table: "MoffPreference",
                newName: "PreferenceId");

            migrationBuilder.RenameColumn(
                name: "moff_preference_id",
                table: "MoffPreference",
                newName: "TempId");

            migrationBuilder.AlterColumn<int>(
                name: "PreferenceId",
                table: "MoffPreference",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "TempId",
                table: "MoffPreference",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_MoffPreference_TempId",
                table: "MoffPreference",
                column: "TempId");
        }
    }
}
