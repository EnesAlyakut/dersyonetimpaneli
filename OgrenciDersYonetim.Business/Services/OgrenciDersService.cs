using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Data.Context;

namespace OgrenciDersYonetim.Business.Services
{
    public class OgrenciDersService : IOgrenciDersService
    {
        private readonly AppDbContext _context;

        public OgrenciDersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<OgrenciDers>> OgrenciyeGoreDerslerGetirAsync(int ogrenciId)
        {
            return await _context.OgrenciDersler
                .Include(od => od.Ders)
                .Where(od => od.OgrenciId == ogrenciId)
                .OrderBy(od => od.Ders.DersAdi)
                .ToListAsync();
        }

        public async Task<List<OgrenciDers>> DerseGoreOgrencilerGetirAsync(int dersId)
        {
            return await _context.OgrenciDersler
                .Include(od => od.Ogrenci)
                .Where(od => od.DersId == dersId)
                .OrderBy(od => od.Ogrenci.Ad)
                .ToListAsync();
        }

        public async Task<bool> AtamaYapAsync(int ogrenciId, int dersId)
        {
            // Zaten atanmış mı kontrol et
            if (await AtamaMevcutMuAsync(ogrenciId, dersId))
                return false;

            var atama = new OgrenciDers
            {
                OgrenciId = ogrenciId,
                DersId = dersId,
                AtamaTarihi = DateTime.Now
            };

            _context.OgrenciDersler.Add(atama);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AtamaKaldirAsync(int ogrenciId, int dersId)
        {
            var atama = await _context.OgrenciDersler
                .FirstOrDefaultAsync(od => od.OgrenciId == ogrenciId && od.DersId == dersId);

            if (atama == null) return false;

            _context.OgrenciDersler.Remove(atama);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AtamaMevcutMuAsync(int ogrenciId, int dersId)
        {
            return await _context.OgrenciDersler
                .AnyAsync(od => od.OgrenciId == ogrenciId && od.DersId == dersId);
        }

        public async Task<List<Ders>> OgrencininAtanmamisDersleriniGetirAsync(int ogrenciId)
        {
            var atanmisDersIdleri = await _context.OgrenciDersler
                .Where(od => od.OgrenciId == ogrenciId)
                .Select(od => od.DersId)
                .ToListAsync();

            return await _context.Dersler
                .Where(d => !atanmisDersIdleri.Contains(d.Id))
                .OrderBy(d => d.DersAdi)
                .ToListAsync();
        }
    }
}
