using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Business.Interfaces
{
    public interface IOgretmenService
    {
        Task<List<Ogretmen>> TumunuGetirAsync();
        Task<Ogretmen?> IdIleGetirAsync(int id);
        Task<Ogretmen?> KullaniciIdIleGetirAsync(int kullaniciId);
        Task<int> EkleAsync(Ogretmen ogretmen);
        Task GuncelleAsync(Ogretmen ogretmen);
        Task SilAsync(int id);

        // Ders atama
        Task<bool> DersAtaAsync(int ogretmenId, int dersId);
        Task<bool> DersAtamaKaldirAsync(int ogretmenId, int dersId);
        Task<List<Ders>> OgretmeninDersleriniGetirAsync(int ogretmenId);
        Task<List<Ders>> OgretmenineAtanmamisDersleriniGetirAsync(int ogretmenId);
        Task<List<int>> OgretmeninOgrenciIdleriniGetirAsync(int ogretmenId);
    }
}
