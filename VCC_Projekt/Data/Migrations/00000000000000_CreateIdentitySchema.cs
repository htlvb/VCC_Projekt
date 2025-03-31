using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace VCC_Projekt.Migrations
{
    /// <inheritdoc />
    public partial class CreateIdentitySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Erstellung der AspNetRoles Tabelle (mit int als RoleId)
            migrationBuilder.CreateTable(
                name: "vcc_AspNetRoles",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetRoles", x => x.Name);
                });

            // Erstellung der vcc_AspNetUsers Tabelle (mit string als UserId)
            migrationBuilder.CreateTable(
                name: "vcc_AspNetUsers",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Firstname = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Lastname = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    Gruppe_GruppenID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetUsers", x => x.UserName);
                    table.ForeignKey(
                        name: "FK_vcc_AspNetUsers_vcc_gruppe_Gruppe_GruppenID",
                        column: x => x.Gruppe_GruppenID,
                        principalTable: "vcc_gruppe",
                        principalColumn: "GruppenID",
                        onDelete: ReferentialAction.Cascade);
                });

            // Erstellung der vcc_AspNetUserRoles Tabelle
            migrationBuilder.CreateTable(
                name: "vcc_AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(256)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false) // RoleId als int
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_vcc_AspNetUserRoles_vcc_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "vcc_AspNetRoles",
                        principalColumn: "Name",  // Bezug auf die 'Id' Spalte von vcc_AspNetRoles
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vcc_AspNetUserRoles_vcc_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "vcc_AspNetUsers",
                        principalColumn: "UserName", // Bezug auf die 'UserName' Spalte von vcc_AspNetUsers
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "vcc_AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_vcc_AspNetUserRoles_RoleId",
                table: "vcc_AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "vcc_AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "vcc_AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vcc_AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "vcc_AspNetRoles");

            migrationBuilder.DropTable(
                name: "vcc_AspNetUsers");
        }
    }

}