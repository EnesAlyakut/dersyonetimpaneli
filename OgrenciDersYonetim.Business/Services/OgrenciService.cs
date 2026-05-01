using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Data.Context;

namespace OgrenciDersYonetim.Business.Services
{
    public class OgrenciService : IOgrenciService
    {
        private readonly AppDbContext _context;

        public OgrenciService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ogrenci>> TumunuGetirAsync()
        {
            return await _context.Ogrenciler
                .Include(o => o.OgrenciDersler)
                    .ThenInclude(od => od.Ders)
                .OrderBy(o => o.Ad)
                .ToListAsync();
        }

        public async Task<List<Ogrenci>> AraAsync(string aramaMetni)
        {
            if (string.IsNullOrWhiteSpace(aramaMetni))
                return await TumunuGetirAsync();

            var ara = aramaMetni.ToLower().Trim();
            return await _context.Ogrenciler
                .Include(o => o.OgrenciDersler)
                    .ThenInclude(od => od.Ders)
                .Where(o =>
                    o.Ad.ToLower().Contains(ara) ||
                    o.Soyad.ToLower().Contains(ara) ||
                    o.OgrenciNo.ToLower().Contains(ara) ||
                    (o.Email != null && o.Email.ToLower().Contains(ara)))
                .OrderBy(o => o.Ad)
                .ToListAsync();
        }

        public async Task<Ogrenci?> IdIleGetirAsync(int id)
        {
            return await _context.Ogrenciler
                .Include(o => o.OgrenciDersler)
                    .ThenInclude(od => od.Ders)
                .Include(o => o.Kullanici)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Ogrenci?> KullaniciIdIleGetirAsync(int kullaniciId)
        {
            return await _context.Ogrenciler
                .Include(o => o.OgrenciDersler)
                    .ThenInclude(od => od.Ders)
                .FirstOrDefaultAsync(o => o.KullaniciId == kullaniciId);
        }

        public async Task<int> EkleAsync(Ogrenci ogrenci)
        {
            ogrenci.KayitTarihi = DateTime.Now;
            _context.Ogrenciler.Add(ogrenci);
            await _context.SaveChangesAsync();
            return ogrenci.Id;
        }

        public async Task GuncelleAsync(Ogrenci ogrenci)
        {
            _context.Ogrenciler.Update(ogrenci);
            await _context.SaveChangesAsync();
        }

        public async Task SilAsync(int id)
        {
            var ogrenci = await _context.Ogrenciler.FindAsync(id);
            if (ogrenci != null)
            {
                _context.Ogrenciler.Remove(ogrenci);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> OgrenciNoMevcutMuAsync(string ogrenciNo, int? haricId = null)
        {
            return await _context.Ogrenciler.AnyAsync(o =>
                o.OgrenciNo == ogrenciNo &&
                (haricId == null || o.Id != haricId));
        }

        public async Task<int> ToplamSayiAsync()
        {
            return await _context.Ogrenciler.CountAsync();
        }
    }
}
