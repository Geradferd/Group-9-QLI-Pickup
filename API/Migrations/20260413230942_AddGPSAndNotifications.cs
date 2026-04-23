using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class AddGPSAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GPS_Track_Point_Drivers_DriverId",
                table: "GPS_Track_Point");

            migrationBuilder.DropForeignKey(
                name: "FK_GPS_Track_Point_Trips_TripId",
                table: "GPS_Track_Point");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Trips_TripId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_RecipientUserId",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GPS_Track_Point",
                table: "GPS_Track_Point");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "GPS_Track_Point",
                newName: "GPS_TrackPoints");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_TripId",
                table: "Notifications",
                newName: "IX_Notifications_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_RecipientUserId_CreatedAt",
                table: "Notifications",
                newName: "IX_Notifications_RecipientUserId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_DeliveryStatus",
                table: "Notifications",
                newName: "IX_Notifications_DeliveryStatus");

            migrationBuilder.RenameIndex(
                name: "IX_GPS_Track_Point_TripId",
                table: "GPS_TrackPoints",
                newName: "IX_GPS_TrackPoints_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_GPS_Track_Point_DriverId_DeviceTimestamp",
                table: "GPS_TrackPoints",
                newName: "IX_GPS_TrackPoints_DriverId_DeviceTimestamp");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryStatus",
                table: "Notifications",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Channel",
                table: "Notifications",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GPS_TrackPoints",
                table: "GPS_TrackPoints",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OperatingHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    day = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    startTime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    endTime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    enabledFlag = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingHours", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SpecialSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    date = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    specialStartTime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    specialEndTime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    closedFlag = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecialSchedules_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialSchedules_UserId",
                table: "SpecialSchedules",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GPS_TrackPoints_Drivers_DriverId",
                table: "GPS_TrackPoints",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GPS_TrackPoints_Trips_TripId",
                table: "GPS_TrackPoints",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Trips_TripId",
                table: "Notifications",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Users_RecipientUserId",
                table: "Notifications",
                column: "RecipientUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GPS_TrackPoints_Drivers_DriverId",
                table: "GPS_TrackPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_GPS_TrackPoints_Trips_TripId",
                table: "GPS_TrackPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Trips_TripId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Users_RecipientUserId",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "OperatingHours");

            migrationBuilder.DropTable(
                name: "SpecialSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GPS_TrackPoints",
                table: "GPS_TrackPoints");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "GPS_TrackPoints",
                newName: "GPS_Track_Point");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TripId",
                table: "Notification",
                newName: "IX_Notification_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_RecipientUserId_CreatedAt",
                table: "Notification",
                newName: "IX_Notification_RecipientUserId_CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_DeliveryStatus",
                table: "Notification",
                newName: "IX_Notification_DeliveryStatus");

            migrationBuilder.RenameIndex(
                name: "IX_GPS_TrackPoints_TripId",
                table: "GPS_Track_Point",
                newName: "IX_GPS_Track_Point_TripId");

            migrationBuilder.RenameIndex(
                name: "IX_GPS_TrackPoints_DriverId_DeviceTimestamp",
                table: "GPS_Track_Point",
                newName: "IX_GPS_Track_Point_DriverId_DeviceTimestamp");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Notification",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryStatus",
                table: "Notification",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "Channel",
                table: "Notification",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GPS_Track_Point",
                table: "GPS_Track_Point",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GPS_Track_Point_Drivers_DriverId",
                table: "GPS_Track_Point",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GPS_Track_Point_Trips_TripId",
                table: "GPS_Track_Point",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Trips_TripId",
                table: "Notification",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_RecipientUserId",
                table: "Notification",
                column: "RecipientUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
