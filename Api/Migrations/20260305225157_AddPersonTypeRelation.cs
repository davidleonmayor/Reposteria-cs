using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonTypeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PersonType",
                table: "Person",
                newName: "PersonTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Person_PersonTypeId",
                table: "Person",
                column: "PersonTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_PersonType_PersonTypeId",
                table: "Person",
                column: "PersonTypeId",
                principalTable: "PersonType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Person_PersonType_PersonTypeId",
                table: "Person");

            migrationBuilder.DropIndex(
                name: "IX_Person_PersonTypeId",
                table: "Person");

            migrationBuilder.RenameColumn(
                name: "PersonTypeId",
                table: "Person",
                newName: "PersonType");
        }
    }
}
