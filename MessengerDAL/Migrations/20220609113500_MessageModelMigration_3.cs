using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessengerDAL.Migrations
{
    public partial class MessageModelMigration_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_DestinationId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_DestinationId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DestinationChatId",
                table: "Messages",
                column: "DestinationChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_DestinationChatId",
                table: "Messages",
                column: "DestinationChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Chats_DestinationChatId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_DestinationChatId",
                table: "Messages");

            migrationBuilder.AddColumn<Guid>(
                name: "DestinationId",
                table: "Messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DestinationId",
                table: "Messages",
                column: "DestinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Chats_DestinationId",
                table: "Messages",
                column: "DestinationId",
                principalTable: "Chats",
                principalColumn: "Id");
        }
    }
}
