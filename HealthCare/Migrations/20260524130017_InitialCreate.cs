using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HealthCare.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Caregivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Specialty = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Caregivers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CaregiverId = table.Column<int>(type: "integer", nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsBooked = table.Column<bool>(type: "boolean", nullable: false),
                    BookedByPatientId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentSlots_Caregivers_CaregiverId",
                        column: x => x.CaregiverId,
                        principalTable: "Caregivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentSlots_Patients_BookedByPatientId",
                        column: x => x.BookedByPatientId,
                        principalTable: "Patients",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Caregivers",
                columns: new[] { "Id", "CreatedAt", "Name", "Specialty" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dr. Anna Andersson", "Allmänmedicin" },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dr. Björn Berg", "Kardiologi" },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dr. Cecilia Carlsson", "Dermatologi" }
                });

            migrationBuilder.InsertData(
                table: "AppointmentSlots",
                columns: new[] { "Id", "BookedByPatientId", "CaregiverId", "EndTime", "IsBooked", "StartTime" },
                values: new object[,]
                {
                    { 1, null, 1, new DateTime(2026, 6, 1, 10, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 6, 1, 9, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, null, 1, new DateTime(2026, 6, 1, 12, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 6, 1, 11, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, null, 2, new DateTime(2026, 6, 2, 10, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 6, 2, 9, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_BookedByPatientId",
                table: "AppointmentSlots",
                column: "BookedByPatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSlots_CaregiverId",
                table: "AppointmentSlots",
                column: "CaregiverId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentSlots");

            migrationBuilder.DropTable(
                name: "Caregivers");

            migrationBuilder.DropTable(
                name: "Patients");
        }
    }
}
