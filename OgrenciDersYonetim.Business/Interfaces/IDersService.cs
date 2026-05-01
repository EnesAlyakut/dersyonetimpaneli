using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Business.Interfaces
{
    public interface IDersService
    {
        Task<List<Ders>> TumunuGetirAsync();
        Task<Ders?> IdIleGetirAsync(int id);
        Task<int> EkleAsync(Ders ders);
        Task GuncelleAsync(Ders ders);
        Task SilAsync(int id);
        Task<bool> DersKoduMevcutMuAsync(string dersKodu, int? haricId = null);
        Task<int> ToplamSayiAsync();
    }
}
