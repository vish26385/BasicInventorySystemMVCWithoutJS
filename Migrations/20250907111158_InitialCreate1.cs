using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALLINONEPROJECTWITHOUTJS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "PartyMasters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentStock",
                table: "ItemMasters",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "PartyMasters");

            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "ItemMasters");
        }
    }
}
