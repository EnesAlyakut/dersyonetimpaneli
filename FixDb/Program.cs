using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Data.Context;
using OgrenciDersYonetim.Core.Entities;

var dbPath = @"C:\Users\enesa\OneDrive\Masaüstü\proje ödevi\OgrenciDersYonetim.Web\OgrenciDersYonetim.db";

var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite($"Data Source={dbPath}")
    .Options;

using var ctx = new AppDbContext(options);

Console.WriteLine("=== VERİTABANI DURUM RAPORU ===\n");

var kullanicilar = await ctx.Kullanicilar.ToListAsync();
Console.WriteLine($"Toplam kullanıcı: {kullanicilar.Count}");
foreach (var k in kullanicilar)
    Console.WriteLine($"  [{k.Id}] {k.KullaniciAdi} | Rol: {k.Rol}");

Console.WriteLine();
var ogretmenler = await ctx.Ogretmenler.ToListAsync();
Console.WriteLine($"Toplam öğretmen kaydı: {ogretmenler.Count}");
foreach (var o in ogretmenler)
    Console.WriteLine($"  [{o.Id}] {o.Ad} {o.Soyad} | KullaniciId: {o.KullaniciId}");

Console.WriteLine();
Console.WriteLine("=== SORUNLU KAYITLAR ===");
var ogretmenKullanicilari = kullanicilar.Where(k => k.Rol == "Ogretmen").ToList();
var linkedKullaniciIdleri = ogretmenler.Select(o => o.KullaniciId).ToList();

var eksikProfiller = ogretmenKullanicilari
    .Where(k => !linkedKullaniciIdleri.Contains(k.Id))
    .ToList();

if (eksikProfiller.Count == 0)
{
    Console.WriteLine("Sorun yok: Tüm Ogretmen kullanıcılarının Ogretmenler tablosunda kaydı var.");
}
else
{
    Console.WriteLine($"⚠️  {eksikProfiller.Count} öğretmen kullanıcısının profil kaydı EKSİK:");
    foreach (var k in eksikProfiller)
    {
        Console.WriteLine($"  [{k.Id}] {k.KullaniciAdi} ({k.Ad} {k.Soyad}) → Ogretmenler tablosunda KullaniciId={k.Id} yok!");
    }

    Console.WriteLine("\nEksik profiller otomatik oluşturulsun mu? (e/h): ");
    var cevap = Console.ReadLine();
    if (cevap?.ToLower() == "e")
    {
        foreach (var k in eksikProfiller)
        {
            var yeniOgretmen = new Ogretmen
            {
                Ad = k.Ad,
                Soyad = k.Soyad,
                Email = k.Email ?? "",
                KullaniciId = k.Id,
                KayitTarihi = DateTime.Now
            };
            ctx.Ogretmenler.Add(yeniOgretmen);
            Console.WriteLine($"  ✅ {k.Ad} {k.Soyad} için Ogretmen kaydı oluşturuluyor...");
        }
        await ctx.SaveChangesAsync();
        Console.WriteLine("Kayıtlar oluşturuldu.");
    }
}

Console.WriteLine();
Console.WriteLine("=== ŞİFRE HASH DÜZELTMESİ ===");
var correctHash = "3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121";
foreach (var k in kullanicilar)
    k.SifreHash = correctHash;
await ctx.SaveChangesAsync();
Console.WriteLine($"✅ {kullanicilar.Count} kullanıcının şifresi '123456' olarak sıfırlandı.");
Console.WriteLine("\nİşlem tamamlandı.");
