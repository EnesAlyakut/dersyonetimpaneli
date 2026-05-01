using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OgrenciDersYonetim.Data.Migrations
{
    /// <inheritdoc />
    public partial class IlkMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Dersler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DersAdi = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DersKodu = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Kredi = table.Column<int>(type: "INTEGER", nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Ogretmen = table.Column<string>(type: "TEXT", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dersler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciAdi = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    SifreHash = table.Column<string>(type: "TEXT", nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Rol = table.Column<string>(type: "TEXT", nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ogrenciler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OgrenciNo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "TEXT", maxLength: 15, nullable: true),
                    KayitTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    KullaniciId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ogrenciler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ogrenciler_Kullanicilar_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "Kullanicilar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "OgrenciDersler",
                columns: table => new
                {
                    OgrenciId = table.Column<int>(type: "INTEGER", nullable: false),
                    DersId = table.Column<int>(type: "INTEGER", nullable: false),
                    AtamaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Not = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OgrenciDersler", x => new { x.OgrenciId, x.DersId });
                    table.ForeignKey(
                        name: "FK_OgrenciDersler_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OgrenciDersler_Ogrenciler_OgrenciId",
                        column: x => x.OgrenciId,
                        principalTable: "Ogrenciler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Dersler",
                columns: new[] { "Id", "Aciklama", "DersAdi", "DersKodu", "EklenmeTarihi", "Kredi", "Ogretmen" },
                values: new object[,]
                {
                    { 1, "Temel matematik dersi", "Matematik", "MAT101", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Prof. Dr. Ali Veli" },
                    { 2, "Temel fizik dersi", "Fizik", "FIZ101", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Doç. Dr. Ayşe Kaya" },
                    { 3, "Programlamaya giriş", "Bilgisayar Bilimi", "BIL101", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Dr. Mehmet Can" }
                });

            migrationBuilder.InsertData(
                table: "Kullanicilar",
                columns: new[] { "Id", "Ad", "Email", "KayitTarihi", "KullaniciAdi", "Rol", "SifreHash", "Soyad" },
                values: new object[,]
                {
                    { 1, "Sistem", "admin@okul.edu.tr", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin", "Admin", "ea8c6f6a71f8fef5e4e96765bfd2c3f0e7bdf6e8c0d9e8f8a1b2c3d4e5f6a7b8", "Yöneticisi" },
                    { 2, "Ahmet", "ogretmen1@okul.edu.tr", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "ogretmen1", "Ogretmen", "ea8c6f6a71f8fef5e4e96765bfd2c3f0e7bdf6e8c0d9e8f8a1b2c3d4e5f6a7b8", "Yılmaz" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Email",
                table: "Kullanicilar",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_KullaniciAdi",
                table: "Kullanicilar",
                column: "KullaniciAdi",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OgrenciDersler_DersId",
                table: "OgrenciDersler",
                column: "DersId");

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_KullaniciId",
                table: "Ogrenciler",
                column: "KullaniciId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ogrenciler_OgrenciNo",
                table: "Ogrenciler",
                column: "OgrenciNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OgrenciDersler");

            migrationBuilder.DropTable(
                name: "Dersler");

            migrationBuilder.DropTable(
                name: "Ogrenciler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
