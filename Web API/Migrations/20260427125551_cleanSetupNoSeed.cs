using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_API.Migrations
{
    /// <inheritdoc />
    public partial class cleanSetupNoSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Trainee_userID",
                table: "Trainee",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_userID",
                table: "Notification",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Instructor_userID",
                table: "Instructor",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coordinator_userID",
                table: "Coordinator",
                column: "userID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Coordinator_AspNetUsers_userID",
                table: "Coordinator",
                column: "userID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructor_AspNetUsers_userID",
                table: "Instructor",
                column: "userID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_userID",
                table: "Notification",
                column: "userID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainee_AspNetUsers_userID",
                table: "Trainee",
                column: "userID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coordinator_AspNetUsers_userID",
                table: "Coordinator");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructor_AspNetUsers_userID",
                table: "Instructor");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_userID",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Trainee_AspNetUsers_userID",
                table: "Trainee");

            migrationBuilder.DropIndex(
                name: "IX_Trainee_userID",
                table: "Trainee");

            migrationBuilder.DropIndex(
                name: "IX_Notification_userID",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Instructor_userID",
                table: "Instructor");

            migrationBuilder.DropIndex(
                name: "IX_Coordinator_userID",
                table: "Coordinator");
        }
    }
}
