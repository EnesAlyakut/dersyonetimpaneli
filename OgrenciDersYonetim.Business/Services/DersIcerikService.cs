using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Data.Context;

namespace OgrenciDersYonetim.Business.Services
{
    public class DersIcerikService : IDersIcerikService
    {
        private readonly AppDbContext _context;

        public DersIcerikService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DersIcerik>> DersIleGetirAsync(int dersId)
        {
            return await _context.DersIcerikler
                .Where(di => di.DersId == dersId)
                .OrderByDescending(di => di.EklenmeTarihi)
                .ToListAsync();
        }

        public async Task<DersIcerik?> IdIleGetirAsync(int id)
        {
            return await _context.DersIcerikler
                .Include(di => di.Ders)
                .FirstOrDefaultAsync(di => di.Id == id);
        }

        public async Task EkleAsync(DersIcerik icerik)
        {
            icerik.EklenmeTarihi = DateTime.Now;
            _context.DersIcerikler.Add(icerik);
            await _context.SaveChangesAsync();
        }

        public async Task GuncelleAsync(DersIcerik icerik)
        {
            _context.DersIcerikler.Update(icerik);
            await _context.SaveChangesAsync();
        }

        public async Task SilAsync(int id)
        {
            var icerik = await _context.DersIcerikler.FindAsync(id);
            if (icerik != null)
            {
                _context.DersIcerikler.Remove(icerik);
                await _context.SaveChangesAsync();
            }
        }
    }
}
