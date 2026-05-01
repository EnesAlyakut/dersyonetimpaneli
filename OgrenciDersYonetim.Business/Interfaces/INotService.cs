using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Business.Interfaces
{
    public interface INotService
    {
        Task<List<Not>> OgrenciIleGetirAsync(int ogrenciId);
        Task<List<Not>> OgrenciVeDersIleGetirAsync(int ogrenciId, int dersId);
        Task<List<Not>> DersIleGetirAsync(int dersId);
        Task<Not?> IdIleGetirAsync(int id);
        Task<int> EkleAsync(Not not);
        Task GuncelleAsync(Not not);
        Task SilAsync(int id);
    }
}
