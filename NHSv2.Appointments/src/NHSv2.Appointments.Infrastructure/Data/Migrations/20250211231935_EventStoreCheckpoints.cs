using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NHSv2.Appointments.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class EventStoreCheckpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventStoreCheckpoints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StreamName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Position = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStoreCheckpoints", x => x.Id);
                });
            
            migrationBuilder.InsertData(
                table: "EventStoreCheckpoints",
                columns: new[] { "Id", "StreamName", "Position", "CreatedAt", "UpdatedAt" },
                values: new object[] { Guid.NewGuid(), "appointments", 0L, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventStoreCheckpoints");
        }
    }
}
