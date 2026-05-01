using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Business.Interfaces
{
    public interface IOgrenciService
    {
        Task<List<Ogrenci>> TumunuGetirAsync();
        Task<List<Ogrenci>> AraAsync(string aramaMetni);
        Task<Ogrenci?> IdIleGetirAsync(int id);
        Task<Ogrenci?> KullaniciIdIleGetirAsync(int kullaniciId);
        Task<int> EkleAsync(Ogrenci ogrenci);
        Task GuncelleAsync(Ogrenci ogrenci);
        Task SilAsync(int id);
        Task<bool> OgrenciNoMevcutMuAsync(string ogrenciNo, int? haricId = null);
        Task<int> ToplamSayiAsync();
    }
}
