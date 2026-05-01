using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Data.Context;

namespace OgrenciDersYonetim.Business.Services
{
    public class NotService : INotService
    {
        private readonly AppDbContext _context;

        public NotService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Not>> OgrenciIleGetirAsync(int ogrenciId)
        {
            return await _context.Notlar
                .Include(n => n.Ders)
                .Include(n => n.Ogrenci)
                .Where(n => n.OgrenciId == ogrenciId)
                .OrderByDescending(n => n.Tarih)
                .ToListAsync();
        }

        public async Task<List<Not>> OgrenciVeDersIleGetirAsync(int ogrenciId, int dersId)
        {
            return await _context.Notlar
                .Include(n => n.Ders)
                .Include(n => n.Ogrenci)
                .Where(n => n.OgrenciId == ogrenciId && n.DersId == dersId)
                .OrderByDescending(n => n.Tarih)
                .ToListAsync();
        }

        public async Task<List<Not>> DersIleGetirAsync(int dersId)
        {
            return await _context.Notlar
                .Include(n => n.Ogrenci)
                .Include(n => n.Ders)
                .Where(n => n.DersId == dersId)
                .OrderBy(n => n.Ogrenci.Ad)
                .ToListAsync();
        }

        public async Task<Not?> IdIleGetirAsync(int id)
        {
            return await _context.Notlar
                .Include(n => n.Ogrenci)
                .Include(n => n.Ders)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<int> EkleAsync(Not not)
        {
            not.Tarih = DateTime.Now;
            _context.Notlar.Add(not);
            await _context.SaveChangesAsync();
            return not.Id;
        }

        public async Task GuncelleAsync(Not not)
        {
            _context.Notlar.Update(not);
            await _context.SaveChangesAsync();
        }

        public async Task SilAsync(int id)
        {
            var not = await _context.Notlar.FindAsync(id);
            if (not != null)
            {
                _context.Notlar.Remove(not);
                await _context.SaveChangesAsync();
            }
        }
    }
}
