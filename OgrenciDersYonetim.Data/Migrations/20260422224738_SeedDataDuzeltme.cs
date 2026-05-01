using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OgrenciDersYonetim.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataDuzeltme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "SifreHash", "Soyad" },
                values: new object[] { "3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121", "Yoneticisi" });

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "SifreHash", "Soyad" },
                values: new object[] { "3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121", "Yilmaz" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "SifreHash", "Soyad" },
                values: new object[] { "ea8c6f6a71f8fef5e4e96765bfd2c3f0e7bdf6e8c0d9e8f8a1b2c3d4e5f6a7b8", "Yöneticisi" });

            migrationBuilder.UpdateData(
                table: "Kullanicilar",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "SifreHash", "Soyad" },
                values: new object[] { "ea8c6f6a71f8fef5e4e96765bfd2c3f0e7bdf6e8c0d9e8f8a1b2c3d4e5f6a7b8", "Yılmaz" });
        }
    }
}
