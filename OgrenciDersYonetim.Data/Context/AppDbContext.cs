using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Data.Context
{
    /// <summary>
    /// Entity Framework Core veritabanı bağlam sınıfı.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet tanımlamaları
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Ogrenci> Ogrenciler { get; set; }
        public DbSet<Ders> Dersler { get; set; }
        public DbSet<OgrenciDers> OgrenciDersler { get; set; }
        public DbSet<Ogretmen> Ogretmenler { get; set; }
        public DbSet<OgretmenDers> OgretmenDersler { get; set; }
        public DbSet<Not> Notlar { get; set; }
        public DbSet<Devamsizlik> Devamsizliklar { get; set; }
        public DbSet<DersIcerik> DersIcerikler { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // OgrenciDers - Composite Primary Key tanımı (Many-to-Many)
            modelBuilder.Entity<OgrenciDers>()
                .HasKey(od => new { od.OgrenciId, od.DersId });

            // OgrenciDers - Ogrenci ilişkisi
            modelBuilder.Entity<OgrenciDers>()
                .HasOne(od => od.Ogrenci)
                .WithMany(o => o.OgrenciDersler)
                .HasForeignKey(od => od.OgrenciId)
                .OnDelete(DeleteBehavior.Cascade);

            // OgrenciDers - Ders ilişkisi
            modelBuilder.Entity<OgrenciDers>()
                .HasOne(od => od.Ders)
                .WithMany(d => d.OgrenciDersler)
                .HasForeignKey(od => od.DersId)
                .OnDelete(DeleteBehavior.Cascade);

            // Kullanici - Ogrenci (1-to-1 opsiyonel)
            modelBuilder.Entity<Ogrenci>()
                .HasOne(o => o.Kullanici)
                .WithOne(k => k.OgrenciProfil)
                .HasForeignKey<Ogrenci>(o => o.KullaniciId)
                .OnDelete(DeleteBehavior.SetNull);

            // OgretmenDers - Composite Primary Key (Many-to-Many)
            modelBuilder.Entity<OgretmenDers>()
                .HasKey(od => new { od.OgretmenId, od.DersId });

            modelBuilder.Entity<OgretmenDers>()
                .HasOne(od => od.Ogretmen)
                .WithMany(o => o.OgretmenDersler)
                .HasForeignKey(od => od.OgretmenId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OgretmenDers>()
                .HasOne(od => od.Ders)
                .WithMany(d => d.OgretmenDersler)
                .HasForeignKey(od => od.DersId)
                .OnDelete(DeleteBehavior.Cascade);

            // Not - Ogrenci iliskisi
            modelBuilder.Entity<Not>()
                .HasOne(n => n.Ogrenci)
                .WithMany()
                .HasForeignKey(n => n.OgrenciId)
                .OnDelete(DeleteBehavior.Cascade);

            // Not - Ders iliskisi
            modelBuilder.Entity<Not>()
                .HasOne(n => n.Ders)
                .WithMany()
                .HasForeignKey(n => n.DersId)
                .OnDelete(DeleteBehavior.Cascade);

            // Devamsizlik - Ogrenci iliskisi
            modelBuilder.Entity<Devamsizlik>()
                .HasOne(d => d.Ogrenci)
                .WithMany()
                .HasForeignKey(d => d.OgrenciId)
                .OnDelete(DeleteBehavior.Cascade);

            // Devamsizlik - Ders iliskisi
            modelBuilder.Entity<Devamsizlik>()
                .HasOne(d => d.Ders)
                .WithMany()
                .HasForeignKey(d => d.DersId)
                .OnDelete(DeleteBehavior.Cascade);

            // DersIcerik - Ders iliskisi
            modelBuilder.Entity<DersIcerik>()
                .HasOne(di => di.Ders)
                .WithMany(d => d.DersIcerikler)
                .HasForeignKey(di => di.DersId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique index: KullaniciAdi
            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.KullaniciAdi)
                .IsUnique();

            // Unique index: Email
            modelBuilder.Entity<Kullanici>()
                .HasIndex(k => k.Email)
                .IsUnique();

            // Unique index: OgrenciNo
            modelBuilder.Entity<Ogrenci>()
                .HasIndex(o => o.OgrenciNo)
                .IsUnique();

            // Seed data - Admin kullanicisi
            // Sifre: Admin123! -> SHA256: 3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121
            modelBuilder.Entity<Kullanici>().HasData(new Kullanici
            {
                Id = 1,
                KullaniciAdi = "admin",
                Email = "admin@okul.edu.tr",
                SifreHash = "3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121",
                Ad = "Sistem",
                Soyad = "Yoneticisi",
                Rol = "Admin",
                KayitTarihi = new DateTime(2024, 1, 1)
            });

            // Seed data - Ogretmen
            modelBuilder.Entity<Kullanici>().HasData(new Kullanici
            {
                Id = 2,
                KullaniciAdi = "ogretmen1",
                Email = "ogretmen1@okul.edu.tr",
                SifreHash = "3eb3fe66b31e3b4d10fa70b5cad49c7112294af6ae4e476a1c405155d45aa121",
                Ad = "Ahmet",
                Soyad = "Yilmaz",
                Rol = "Ogretmen",
                KayitTarihi = new DateTime(2024, 1, 1)
            });

            // Seed data - Dersler
            modelBuilder.Entity<Ders>().HasData(
                new Ders
                {
                    Id = 1,
                    DersAdi = "Matematik",
                    DersKodu = "MAT101",
                    Kredi = 4,
                    Aciklama = "Temel matematik dersi",
                    Ogretmen = "Prof. Dr. Ali Veli",
                    EklenmeTarihi = new DateTime(2024, 1, 1)
                },
                new Ders
                {
                    Id = 2,
                    DersAdi = "Fizik",
                    DersKodu = "FIZ101",
                    Kredi = 3,
                    Aciklama = "Temel fizik dersi",
                    Ogretmen = "Doç. Dr. Ayşe Kaya",
                    EklenmeTarihi = new DateTime(2024, 1, 1)
                },
                new Ders
                {
                    Id = 3,
                    DersAdi = "Bilgisayar Bilimi",
                    DersKodu = "BIL101",
                    Kredi = 3,
                    Aciklama = "Programlamaya giriş",
                    Ogretmen = "Dr. Mehmet Can",
                    EklenmeTarihi = new DateTime(2024, 1, 1)
                }
            );
        }
    }
}
