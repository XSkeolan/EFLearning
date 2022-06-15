using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessengerDAL.Migrations
{
    public partial class MessageModelMigration_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_MessageId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_MessageId2",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MessageId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MessageId2",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageId2",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_OriginalMessageId",
                table: "Messages",
                column: "OriginalMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ReplyMessageId",
                table: "Messages",
                column: "ReplyMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_OriginalMessageId",
                table: "Messages",
                column: "OriginalMessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_ReplyMessageId",
                table: "Messages",
                column: "ReplyMessageId",
                principalTable: "Messages",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_OriginalMessageId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_ReplyMessageId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_OriginalMessageId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ReplyMessageId",
                table: "Messages");

            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "Messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MessageId2",
                table: "Messages",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageId",
                table: "Messages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageId2",
                table: "Messages",
                column: "MessageId2");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_MessageId",
                table: "Messages",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_MessageId2",
                table: "Messages",
                column: "MessageId2",
                principalTable: "Messages",
                principalColumn: "Id");
        }
    }
}
