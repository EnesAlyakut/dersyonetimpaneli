using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Data.Context;
using System.Security.Cryptography;
using System.Text;

namespace OgrenciDersYonetim.Business.Services
{
    public class KullaniciService : IKullaniciService
    {
        private readonly AppDbContext _context;

        public KullaniciService(AppDbContext context)
        {
            _context = context;
        }

        public string SifreyiHashle(string sifre)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(sifre));
            return Convert.ToHexString(hashBytes).ToLower();
        }

        public async Task<Kullanici?> GirisYapAsync(string kullaniciAdi, string sifre)
        {
            var sifreHash = SifreyiHashle(sifre);
            return await _context.Kullanicilar
                .Include(k => k.OgrenciProfil)
                .FirstOrDefaultAsync(k =>
                    k.KullaniciAdi == kullaniciAdi &&
                    k.SifreHash == sifreHash);
        }

        public async Task<bool> KayitOlAsync(Kullanici kullanici, string sifre)
        {
            // Benzersizlik kontrolü
            var mevcutKullanici = await _context.Kullanicilar
                .AnyAsync(k => k.KullaniciAdi == kullanici.KullaniciAdi || k.Email == kullanici.Email);

            if (mevcutKullanici) return false;

            kullanici.SifreHash = SifreyiHashle(sifre);
            kullanici.KayitTarihi = DateTime.Now;

            _context.Kullanicilar.Add(kullanici);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Kullanici?> IdIleGetirAsync(int id)
        {
            return await _context.Kullanicilar
                .Include(k => k.OgrenciProfil)
                .FirstOrDefaultAsync(k => k.Id == id);
        }

        public async Task<List<Kullanici>> TumunuGetirAsync()
        {
            return await _context.Kullanicilar
                .Include(k => k.OgrenciProfil)
                .OrderBy(k => k.Ad)
                .ToListAsync();
        }

        public async Task<bool> KullaniciAdiMevcutMuAsync(string kullaniciAdi)
        {
            return await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == kullaniciAdi);
        }

        public async Task<bool> EmailMevcutMuAsync(string email)
        {
            return await _context.Kullanicilar.AnyAsync(k => k.Email == email);
        }

        public async Task GuncelleAsync(Kullanici kullanici)
        {
            _context.Kullanicilar.Update(kullanici);
            await _context.SaveChangesAsync();
        }

        public async Task SifreDegistirAsync(int kullaniciId, string yeniSifre)
        {
            var kullanici = await _context.Kullanicilar.FindAsync(kullaniciId);
            if (kullanici != null)
            {
                kullanici.SifreHash = SifreyiHashle(yeniSifre);
                await _context.SaveChangesAsync();
            }
        }
    }
}
