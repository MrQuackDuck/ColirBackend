using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class CascadeFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LastTimeUserReadChats_Rooms_RoomId",
                table: "LastTimeUserReadChats");

            migrationBuilder.DropForeignKey(
                name: "FK_LastTimeUserReadChats_Users_UserId",
                table: "LastTimeUserReadChats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_RepliedMessageId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Rooms_RoomId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_AuthorId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_OwnerId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_UserToRoom_Rooms_RoomId",
                table: "UserToRoom");

            migrationBuilder.DropForeignKey(
                name: "FK_UserToRoom_Users_UserId",
                table: "UserToRoom");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_Guid",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserToRoom",
                table: "UserToRoom");

            migrationBuilder.RenameTable(
                name: "UserToRoom",
                newName: "UsersToRooms");

            migrationBuilder.RenameIndex(
                name: "IX_UserToRoom_UserId",
                table: "UsersToRooms",
                newName: "IX_UsersToRooms_UserId");

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "Rooms",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "Rooms",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "RoomId",
                table: "Messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersToRooms",
                table: "UsersToRooms",
                columns: new[] { "RoomId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Guid",
                table: "Rooms",
                column: "Guid",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LastTimeUserReadChats_Rooms_RoomId",
                table: "LastTimeUserReadChats",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_LastTimeUserReadChats_Users_UserId",
                table: "LastTimeUserReadChats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_RepliedMessageId",
                table: "Messages",
                column: "RepliedMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Rooms_RoomId",
                table: "Messages",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_AuthorId",
                table: "Messages",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_OwnerId",
                table: "Rooms",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersToRooms_Rooms_RoomId",
                table: "UsersToRooms",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersToRooms_Users_UserId",
                table: "UsersToRooms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LastTimeUserReadChats_Rooms_RoomId",
                table: "LastTimeUserReadChats");

            migrationBuilder.DropForeignKey(
                name: "FK_LastTimeUserReadChats_Users_UserId",
                table: "LastTimeUserReadChats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_RepliedMessageId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Rooms_RoomId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Users_AuthorId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Users_OwnerId",
                table: "Rooms");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersToRooms_Rooms_RoomId",
                table: "UsersToRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersToRooms_Users_UserId",
                table: "UsersToRooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_Guid",
                table: "Rooms");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersToRooms",
                table: "UsersToRooms");

            migrationBuilder.RenameTable(
                name: "UsersToRooms",
                newName: "UserToRoom");

            migrationBuilder.RenameIndex(
                name: "IX_UsersToRooms_UserId",
                table: "UserToRoom",
                newName: "IX_UserToRoom_UserId");

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "Rooms",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Rooms",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647);

            migrationBuilder.AlterColumn<string>(
                name: "Guid",
                table: "Rooms",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(36)",
                oldMaxLength: 36);

            migrationBuilder.AlterColumn<long>(
                name: "RoomId",
                table: "Messages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserToRoom",
                table: "UserToRoom",
                columns: new[] { "RoomId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Guid",
                table: "Rooms",
                column: "Guid",
                unique: true,
                filter: "[Guid] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_LastTimeUserReadChats_Rooms_RoomId",
                table: "LastTimeUserReadChats",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LastTimeUserReadChats_Users_UserId",
                table: "LastTimeUserReadChats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_RepliedMessageId",
                table: "Messages",
                column: "RepliedMessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Rooms_RoomId",
                table: "Messages",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Users_AuthorId",
                table: "Messages",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Users_OwnerId",
                table: "Rooms",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserToRoom_Rooms_RoomId",
                table: "UserToRoom",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserToRoom_Users_UserId",
                table: "UserToRoom",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
