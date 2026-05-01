using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciDersYonetim.Data.Migrations
{
    /// <inheritdoc />
    public partial class OgrenciNavProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devamsizliklar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OgrenciId = table.Column<int>(type: "INTEGER", nullable: false),
                    DersId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    GirenKullaniciId = table.Column<int>(type: "INTEGER", nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devamsizliklar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devamsizliklar_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Devamsizliklar_Ogrenciler_OgrenciId",
                        column: x => x.OgrenciId,
                        principalTable: "Ogrenciler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OgrenciId = table.Column<int>(type: "INTEGER", nullable: false),
                    DersId = table.Column<int>(type: "INTEGER", nullable: false),
                    NotDegeri = table.Column<double>(type: "REAL", nullable: false),
                    NotTuru = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    GirenKullaniciId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notlar_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notlar_Ogrenciler_OgrenciId",
                        column: x => x.OgrenciId,
                        principalTable: "Ogrenciler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ogretmenler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Brans = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ogretmenler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ogretmenler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OgretmenDersler",
                columns: table => new
                {
                    OgretmenId = table.Column<int>(type: "INTEGER", nullable: false),
                    DersId = table.Column<int>(type: "INTEGER", nullable: false),
                    AtamaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgretmenDersler", x => new { x.OgretmenId, x.DersId });
                    table.ForeignKey(
                        name: "FK_OgretmenDersler_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OgretmenDersler_Ogretmenler_OgretmenId",
                        column: x => x.OgretmenId,
                        principalTable: "Ogretmenler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devamsizliklar_DersId",
                table: "Devamsizliklar",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_Devamsizliklar_OgrenciId",
                table: "Devamsizliklar",
                column: "OgrenciId");

            migrationBuilder.CreateIndex(
                name: "IX_Notlar_DersId",
                table: "Notlar",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_Notlar_OgrenciId",
                table: "Notlar",
                column: "OgrenciId");

            migrationBuilder.CreateIndex(
                name: "IX_OgretmenDersler_DersId",
                table: "OgretmenDersler",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_Ogretmenler_KullaniciId",
                table: "Ogretmenler",
                column: "KullaniciId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Devamsizliklar");

            migrationBuilder.DropTable(
                name: "Notlar");

            migrationBuilder.DropTable(
                name: "OgretmenDersler");

            migrationBuilder.DropTable(
                name: "Ogretmenler");
        }
    }
}
