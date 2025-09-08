using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ALLINONEPROJECTWITHOUTJS.Migrations
{
    /// <inheritdoc />
    public partial class IniCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseMaster_PartyMasters_PartyMasterId",
                table: "PurchaseMaster");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseMaster_PartyMasterId",
                table: "PurchaseMaster");

            migrationBuilder.DropColumn(
                name: "PartyMasterId",
                table: "PurchaseMaster");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMaster_PartyId",
                table: "PurchaseMaster",
                column: "PartyId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseMaster_PartyMasters_PartyId",
                table: "PurchaseMaster",
                column: "PartyId",
                principalTable: "PartyMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseMaster_PartyMasters_PartyId",
                table: "PurchaseMaster");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseMaster_PartyId",
                table: "PurchaseMaster");

            migrationBuilder.AddColumn<int>(
                name: "PartyMasterId",
                table: "PurchaseMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseMaster_PartyMasterId",
                table: "PurchaseMaster",
                column: "PartyMasterId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseMaster_PartyMasters_PartyMasterId",
                table: "PurchaseMaster",
                column: "PartyMasterId",
                principalTable: "PartyMasters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
