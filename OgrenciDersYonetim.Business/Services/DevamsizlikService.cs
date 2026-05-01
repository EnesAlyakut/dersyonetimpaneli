using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Core.Entities;
using OgrenciDersYonetim.Data.Context;

namespace OgrenciDersYonetim.Business.Services
{
    public class DevamsizlikService : IDevamsizlikService
    {
        private readonly AppDbContext _context;

        public DevamsizlikService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Devamsizlik>> OgrenciIleGetirAsync(int ogrenciId)
        {
            return await _context.Devamsizliklar
                .Include(d => d.Ders)
                .Include(d => d.Ogrenci)
                .Where(d => d.OgrenciId == ogrenciId)
                .OrderByDescending(d => d.Tarih)
                .ToListAsync();
        }

        public async Task<List<Devamsizlik>> OgrenciVeDersIleGetirAsync(int ogrenciId, int dersId)
        {
            return await _context.Devamsizliklar
                .Include(d => d.Ders)
                .Include(d => d.Ogrenci)
                .Where(d => d.OgrenciId == ogrenciId && d.DersId == dersId)
                .OrderByDescending(d => d.Tarih)
                .ToListAsync();
        }

        public async Task<List<Devamsizlik>> DersIleGetirAsync(int dersId)
        {
            return await _context.Devamsizliklar
                .Include(d => d.Ogrenci)
                .Include(d => d.Ders)
                .Where(d => d.DersId == dersId)
                .OrderByDescending(d => d.Tarih)
                .ToListAsync();
        }

        public async Task<Devamsizlik?> IdIleGetirAsync(int id)
        {
            return await _context.Devamsizliklar
                .Include(d => d.Ogrenci)
                .Include(d => d.Ders)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<int> EkleAsync(Devamsizlik devamsizlik)
        {
            devamsizlik.KayitTarihi = DateTime.Now;
            _context.Devamsizliklar.Add(devamsizlik);
            await _context.SaveChangesAsync();
            return devamsizlik.Id;
        }

        public async Task GuncelleAsync(Devamsizlik devamsizlik)
        {
            _context.Devamsizliklar.Update(devamsizlik);
            await _context.SaveChangesAsync();
        }

        public async Task SilAsync(int id)
        {
            var devamsizlik = await _context.Devamsizliklar.FindAsync(id);
            if (devamsizlik != null)
            {
                _context.Devamsizliklar.Remove(devamsizlik);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> OgrenciToplamDevamsizlikAsync(int ogrenciId)
        {
            return await _context.Devamsizliklar
                .Where(d => d.OgrenciId == ogrenciId)
                .CountAsync();
        }
    }
}
