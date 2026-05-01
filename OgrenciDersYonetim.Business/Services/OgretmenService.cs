using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Data.Context;

namespace OgrenciDersYonetim.Business.Services
{
    public class OgretmenService : IOgretmenService
    {
        private readonly AppDbContext _context;

        public OgretmenService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ogretmen>> TumunuGetirAsync()
        {
            return await _context.Ogretmenler
                .Include(o => o.OgretmenDersler)
                    .ThenInclude(od => od.Ders)
                .Include(o => o.Kullanici)
                .OrderBy(o => o.Ad)
                .ToListAsync();
        }

        public async Task<Ogretmen?> IdIleGetirAsync(int id)
        {
            return await _context.Ogretmenler
                .Include(o => o.OgretmenDersler)
                    .ThenInclude(od => od.Ders)
                .Include(o => o.Kullanici)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Ogretmen?> KullaniciIdIleGetirAsync(int kullaniciId)
        {
            return await _context.Ogretmenler
                .Include(o => o.OgretmenDersler)
                    .ThenInclude(od => od.Ders)
                .FirstOrDefaultAsync(o => o.KullaniciId == kullaniciId);
        }

        public async Task<int> EkleAsync(Ogretmen ogretmen)
        {
            ogretmen.KayitTarihi = DateTime.Now;
            _context.Ogretmenler.Add(ogretmen);
            await _context.SaveChangesAsync();
            return ogretmen.Id;
        }

        public async Task GuncelleAsync(Ogretmen ogretmen)
        {
            _context.Ogretmenler.Update(ogretmen);
            await _context.SaveChangesAsync();
        }

        public async Task SilAsync(int id)
        {
            var ogretmen = await _context.Ogretmenler.FindAsync(id);
            if (ogretmen != null)
            {
                _context.Ogretmenler.Remove(ogretmen);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DersAtaAsync(int ogretmenId, int dersId)
        {
            var mevcutAtama = await _context.OgretmenDersler
                .AnyAsync(od => od.OgretmenId == ogretmenId && od.DersId == dersId);

            if (mevcutAtama) return false;

            // Navigation property'leri yükle (EF Core FK constraint için gerekli)
            var ogretmen = await _context.Ogretmenler.FindAsync(ogretmenId);
            var ders = await _context.Dersler.FindAsync(dersId);

            if (ogretmen == null || ders == null) return false;

            var atama = new OgretmenDers
            {
                OgretmenId = ogretmenId,
                Ogretmen = ogretmen,
                DersId = dersId,
                Ders = ders,
                AtamaTarihi = DateTime.Now
            };

            _context.OgretmenDersler.Add(atama);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DersAtamaKaldirAsync(int ogretmenId, int dersId)
        {
            var atama = await _context.OgretmenDersler
                .FirstOrDefaultAsync(od => od.OgretmenId == ogretmenId && od.DersId == dersId);

            if (atama == null) return false;

            _context.OgretmenDersler.Remove(atama);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Ders>> OgretmeninDersleriniGetirAsync(int ogretmenId)
        {
            return await _context.OgretmenDersler
                .Include(od => od.Ders)
                    .ThenInclude(d => d.OgrenciDersler)
                        .ThenInclude(od => od.Ogrenci)
                .Where(od => od.OgretmenId == ogretmenId)
                .Select(od => od.Ders)
                .OrderBy(d => d.DersAdi)
                .ToListAsync();
        }

        public async Task<List<int>> OgretmeninOgrenciIdleriniGetirAsync(int ogretmenId)
        {
            // Öğretmenin tüm derslerine kayıtlı öğrenci ID'leri
            var dersIdleri = await _context.OgretmenDersler
                .Where(od => od.OgretmenId == ogretmenId)
                .Select(od => od.DersId)
                .ToListAsync();

            return await _context.OgrenciDersler
                .Where(od => dersIdleri.Contains(od.DersId))
                .Select(od => od.OgrenciId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Ders>> OgretmenineAtanmamisDersleriniGetirAsync(int ogretmenId)
        {
            var atanmisDersIdleri = await _context.OgretmenDersler
                .Where(od => od.OgretmenId == ogretmenId)
                .Select(od => od.DersId)
                .ToListAsync();

            return await _context.Dersler
                .Where(d => !atanmisDersIdleri.Contains(d.Id))
                .OrderBy(d => d.DersAdi)
                .ToListAsync();
        }
    }
}
