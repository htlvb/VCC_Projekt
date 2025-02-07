using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VCC_Projekt.Migrations
{
    /// <inheritdoc />
    public partial class temporary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_AspNetRoles",
                columns: table => new
                {
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Id = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Beschreibung = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetRoles", x => x.Name);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_AspNetUsers",
                columns: table => new
                {
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Id = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Firstname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Lastname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetUsers", x => x.UserName);
                    table.UniqueConstraint("AK_vcc_AspNetUsers_Id", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_event",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Bezeichnung = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Beginn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Dauer = table.Column<int>(type: "int", nullable: false),
                    StrafminutenProFehlversuch = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_event", x => x.EventID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vcc_AspNetRoleClaims_vcc_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "vcc_AspNetRoles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vcc_AspNetUserClaims_vcc_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "vcc_AspNetUsers",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_vcc_AspNetUserLogins_vcc_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "vcc_AspNetUsers",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_vcc_AspNetUserRoles_vcc_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "vcc_AspNetRoles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vcc_AspNetUserRoles_vcc_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "vcc_AspNetUsers",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "varchar(256)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_vcc_AspNetUserTokens_vcc_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "vcc_AspNetUsers",
                        principalColumn: "UserName",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_gruppe",
                columns: table => new
                {
                    GruppenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Gruppenname = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Event_EventID = table.Column<int>(type: "int", nullable: false),
                    GruppenleiterId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Teilnehmertyp = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_gruppe", x => x.GruppenID);
                    table.ForeignKey(
                        name: "FK_vcc_gruppe_vcc_AspNetUsers_GruppenleiterId",
                        column: x => x.GruppenleiterId,
                        principalTable: "vcc_AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vcc_gruppe_vcc_event_Event_EventID",
                        column: x => x.Event_EventID,
                        principalTable: "vcc_event",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_level",
                columns: table => new
                {
                    LevelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Levelnr = table.Column<int>(type: "int", nullable: false),
                    Angabe_PDF = table.Column<byte[]>(type: "longblob", nullable: false),
                    Event_EventID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_level", x => x.LevelID);
                    table.ForeignKey(
                        name: "FK_vcc_level_vcc_event_Event_EventID",
                        column: x => x.Event_EventID,
                        principalTable: "vcc_event",
                        principalColumn: "EventID",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_UserInGruppe",
                columns: table => new
                {
                    User_UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Gruppe_GruppenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_UserInGruppe", x => new { x.User_UserId, x.Gruppe_GruppenId });
                    table.ForeignKey(
                        name: "FK_vcc_UserInGruppe_vcc_AspNetUsers_User_UserId",
                        column: x => x.User_UserId,
                        principalTable: "vcc_AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vcc_UserInGruppe_vcc_gruppe_Gruppe_GruppenId",
                        column: x => x.Gruppe_GruppenId,
                        principalTable: "vcc_gruppe",
                        principalColumn: "GruppenID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_aufgaben",
                columns: table => new
                {
                    AufgabenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Aufgabennr = table.Column<int>(type: "int", nullable: false),
                    Input_TXT = table.Column<byte[]>(type: "longblob", nullable: false),
                    Ergebnis_TXT = table.Column<byte[]>(type: "longblob", nullable: false),
                    Level_LevelID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_aufgaben", x => x.AufgabenID);
                    table.ForeignKey(
                        name: "FK_vcc_aufgaben_vcc_level_Level_LevelID",
                        column: x => x.Level_LevelID,
                        principalTable: "vcc_level",
                        principalColumn: "LevelID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "vcc_gruppe_absolviert_level",
                columns: table => new
                {
                    Gruppe_GruppeID = table.Column<int>(type: "int", nullable: false),
                    Level_LevelID = table.Column<int>(type: "int", nullable: false),
                    BenoetigteZeit = table.Column<TimeSpan>(type: "time(6)", nullable: false),
                    Fehlversuche = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vcc_gruppe_absolviert_level", x => new { x.Gruppe_GruppeID, x.Level_LevelID });
                    table.ForeignKey(
                        name: "FK_vcc_gruppe_absolviert_level_vcc_gruppe_Gruppe_GruppeID",
                        column: x => x.Gruppe_GruppeID,
                        principalTable: "vcc_gruppe",
                        principalColumn: "GruppenID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_vcc_gruppe_absolviert_level_vcc_level_Gruppe_GruppeID",
                        column: x => x.Gruppe_GruppeID,
                        principalTable: "vcc_level",
                        principalColumn: "LevelID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_vcc_AspNetRoleClaims_RoleId",
                table: "vcc_AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "vcc_AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vcc_AspNetUserClaims_UserId",
                table: "vcc_AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_vcc_AspNetUserLogins_UserId",
                table: "vcc_AspNetUserLogins",
                column: "UserId");

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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vcc_aufgaben_Level_LevelID",
                table: "vcc_aufgaben",
                column: "Level_LevelID");

            migrationBuilder.CreateIndex(
                name: "IX_vcc_gruppe_Event_EventID",
                table: "vcc_gruppe",
                column: "Event_EventID");

            migrationBuilder.CreateIndex(
                name: "IX_vcc_gruppe_GruppenleiterId",
                table: "vcc_gruppe",
                column: "GruppenleiterId");

            migrationBuilder.CreateIndex(
                name: "IX_vcc_level_Event_EventID",
                table: "vcc_level",
                column: "Event_EventID");

            migrationBuilder.CreateIndex(
                name: "IX_vcc_UserInGruppe_Gruppe_GruppenId",
                table: "vcc_UserInGruppe",
                column: "Gruppe_GruppenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vcc_AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "vcc_AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "vcc_AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "vcc_AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "vcc_AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "vcc_aufgaben");

            migrationBuilder.DropTable(
                name: "vcc_gruppe_absolviert_level");

            migrationBuilder.DropTable(
                name: "vcc_UserInGruppe");

            migrationBuilder.DropTable(
                name: "vcc_AspNetRoles");

            migrationBuilder.DropTable(
                name: "vcc_level");

            migrationBuilder.DropTable(
                name: "vcc_gruppe");

            migrationBuilder.DropTable(
                name: "vcc_AspNetUsers");

            migrationBuilder.DropTable(
                name: "vcc_event");
        }
    }
}
