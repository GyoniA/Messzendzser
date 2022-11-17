using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messzendzser.Model.Migrations
{
    public partial class UserReplacedWithIdentityUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "chatroom_associations_user_id",
                table: "chatroom_associations");

            migrationBuilder.DropForeignKey(
                name: "image_chat_message_user_id",
                table: "image_chat_message");

            migrationBuilder.DropForeignKey(
                name: "text_chat_message_user_id",
                table: "text_chat_message");

            migrationBuilder.DropForeignKey(
                name: "voice_chat_message_user_id",
                table: "voice_chat_message");

            migrationBuilder.DropForeignKey(
                name: "voip_credentials_user_id",
                table: "voip_credentials");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.AddForeignKey(
                name: "chatroom_associations_user_id",
                table: "chatroom_associations",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "image_chat_message_user_id",
                table: "image_chat_message",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "text_chat_message_user_id",
                table: "text_chat_message",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "voice_chat_message_user_id",
                table: "voice_chat_message",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "voip_credentials_user_id",
                table: "voip_credentials",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "chatroom_associations_user_id",
                table: "chatroom_associations");

            migrationBuilder.DropForeignKey(
                name: "image_chat_message_user_id",
                table: "image_chat_message");

            migrationBuilder.DropForeignKey(
                name: "text_chat_message_user_id",
                table: "text_chat_message");

            migrationBuilder.DropForeignKey(
                name: "voice_chat_message_user_id",
                table: "voice_chat_message");

            migrationBuilder.DropForeignKey(
                name: "voip_credentials_user_id",
                table: "voip_credentials");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    password = table.Column<string>(type: "varchar(90)", maxLength: 90, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    username = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.AddForeignKey(
                name: "chatroom_associations_user_id",
                table: "chatroom_associations",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "image_chat_message_user_id",
                table: "image_chat_message",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "text_chat_message_user_id",
                table: "text_chat_message",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "voice_chat_message_user_id",
                table: "voice_chat_message",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "voip_credentials_user_id",
                table: "voip_credentials",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
