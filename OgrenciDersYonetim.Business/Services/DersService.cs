using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Data.Context;

namespace OgrenciDersYonetim.Business.Services
{
    public class DersService : IDersService
    {
        private readonly AppDbContext _context;

        public DersService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ders>> TumunuGetirAsync()
        {
            return await _context.Dersler
                .Include(d => d.OgrenciDersler)
                .OrderBy(d => d.DersAdi)
                .ToListAsync();
        }

        public async Task<Ders?> IdIleGetirAsync(int id)
        {
            return await _context.Dersler
                .Include(d => d.OgrenciDersler)
                    .ThenInclude(od => od.Ogrenci)
                .Include(d => d.OgretmenDersler)
                    .ThenInclude(od => od.Ogretmen)
                .Include(d => d.DersIcerikler)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<int> EkleAsync(Ders ders)
        {
            ders.EklenmeTarihi = DateTime.Now;
            _context.Dersler.Add(ders);
            await _context.SaveChangesAsync();
            return ders.Id;
        }

        public async Task GuncelleAsync(Ders ders)
        {
            _context.Dersler.Update(ders);
            await _context.SaveChangesAsync();
        }

        public async Task SilAsync(int id)
        {
            var ders = await _context.Dersler.FindAsync(id);
            if (ders != null)
            {
                _context.Dersler.Remove(ders);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DersKoduMevcutMuAsync(string dersKodu, int? haricId = null)
        {
            return await _context.Dersler.AnyAsync(d =>
                d.DersKodu == dersKodu &&
                (haricId == null || d.Id != haricId));
        }

        public async Task<int> ToplamSayiAsync()
        {
            return await _context.Dersler.CountAsync();
        }
    }
}
