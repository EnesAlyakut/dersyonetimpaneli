using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciDersYonetim.Data.Migrations
{
    /// <inheritdoc />
    public partial class DersIcerikEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DersIcerikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DersId = table.Column<int>(type: "INTEGER", nullable: false),
                    Baslik = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Icerik = table.Column<string>(type: "TEXT", nullable: false),
                    Tur = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    SonTeslimTarihi = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EklenmeTarihi = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EkleyenKullaniciId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DersIcerikler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DersIcerikler_Dersler_DersId",
                        column: x => x.DersId,
                        principalTable: "Dersler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DersIcerikler_DersId",
                table: "DersIcerikler",
                column: "DersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DersIcerikler");
        }
    }
}
