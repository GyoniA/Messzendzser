using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Messzendzser.Model.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "chatroom",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    icon = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_chatroom", x => x.id);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "ChatroomUser",
                columns: table => new
                {
                    ChatroomId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatroomUser", x => new { x.ChatroomId, x.UserId });
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    email = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    username = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(90)", maxLength: 90, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.id);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "whiteboard",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    chatroom_id = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    @event = table.Column<string>(name: "event", type: "varchar(400)", maxLength: 400, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    time = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_whiteboard", x => x.id);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "chatroom_associations",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int(11)", nullable: false),
                    chatroom_id = table.Column<int>(type: "int(11)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => new { x.user_id, x.chatroom_id })
                        .Annotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                    table.ForeignKey(
                        name: "chatroom_associations_chatroom_id",
                        column: x => x.chatroom_id,
                        principalTable: "chatroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "chatroom_associations_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "image_chat_message",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int(11)", nullable: false),
                    sent_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    chatroom_id = table.Column<int>(type: "int(11)", nullable: false),
                    token = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    format = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_image_chat_message", x => x.id);
                    table.ForeignKey(
                        name: "image_chat_message_chatroom_id",
                        column: x => x.chatroom_id,
                        principalTable: "chatroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "image_chat_message_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "text_chat_message",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int(11)", nullable: false),
                    sent_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    chatroom_id = table.Column<int>(type: "int(11)", nullable: false),
                    message = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_text_chat_message", x => x.id);
                    table.ForeignKey(
                        name: "text_chat_message_chatroom_id",
                        column: x => x.chatroom_id,
                        principalTable: "chatroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "text_chat_message_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateTable(
                name: "voice_chat_message",
                columns: table => new
                {
                    id = table.Column<int>(type: "int(11)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<int>(type: "int(11)", nullable: false),
                    sent_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    chatroom_id = table.Column<int>(type: "int(11)", nullable: false),
                    token = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1"),
                    length = table.Column<int>(type: "int(11)", nullable: false),
                    format = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, collation: "latin1_swedish_ci")
                        .Annotation("MySql:CharSet", "latin1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_voice_chat_message", x => x.id);
                    table.ForeignKey(
                        name: "voice_chat_message_chatroom_id",
                        column: x => x.chatroom_id,
                        principalTable: "chatroom",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "voice_chat_message_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "latin1")
                .Annotation("Relational:Collation", "latin1_swedish_ci");

            migrationBuilder.CreateTable(
                name: "voip_credentials",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int(11)", nullable: false),
                    voip_username = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    voip_password = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    voip_realm_hash = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false, collation: "utf8mb4_general_ci")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PRIMARY", x => x.user_id);
                    table.ForeignKey(
                        name: "voip_credentials_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateIndex(
                name: "chatroom_id_idx",
                table: "chatroom_associations",
                column: "chatroom_id");

            migrationBuilder.CreateIndex(
                name: "user_id_idx",
                table: "chatroom_associations",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "image_chat_message_chatroom_id_idx",
                table: "image_chat_message",
                column: "chatroom_id");

            migrationBuilder.CreateIndex(
                name: "image_chat_message_user_id_idx",
                table: "image_chat_message",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "chatroom_id_idx1",
                table: "text_chat_message",
                column: "chatroom_id");

            migrationBuilder.CreateIndex(
                name: "user_id_idx1",
                table: "text_chat_message",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "voice_chat_message_chatroom_id_idx",
                table: "voice_chat_message",
                column: "chatroom_id");

            migrationBuilder.CreateIndex(
                name: "voice_chat_message_user_id_idx",
                table: "voice_chat_message",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chatroom_associations");

            migrationBuilder.DropTable(
                name: "ChatroomUser");

            migrationBuilder.DropTable(
                name: "image_chat_message");

            migrationBuilder.DropTable(
                name: "text_chat_message");

            migrationBuilder.DropTable(
                name: "voice_chat_message");

            migrationBuilder.DropTable(
                name: "voip_credentials");

            migrationBuilder.DropTable(
                name: "whiteboard");

            migrationBuilder.DropTable(
                name: "chatroom");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
