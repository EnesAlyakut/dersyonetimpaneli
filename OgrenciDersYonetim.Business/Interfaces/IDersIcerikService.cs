using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Business.Interfaces
{
    public interface IDersIcerikService
    {
        Task<List<DersIcerik>> DersIleGetirAsync(int dersId);
        Task<DersIcerik?> IdIleGetirAsync(int id);
        Task EkleAsync(DersIcerik icerik);
        Task GuncelleAsync(DersIcerik icerik);
        Task SilAsync(int id);
    }
}
