using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Business.Interfaces
{
    public interface IDevamsizlikService
    {
        Task<List<Devamsizlik>> OgrenciIleGetirAsync(int ogrenciId);
        Task<List<Devamsizlik>> OgrenciVeDersIleGetirAsync(int ogrenciId, int dersId);
        Task<List<Devamsizlik>> DersIleGetirAsync(int dersId);
        Task<Devamsizlik?> IdIleGetirAsync(int id);
        Task<int> EkleAsync(Devamsizlik devamsizlik);
        Task GuncelleAsync(Devamsizlik devamsizlik);
        Task SilAsync(int id);
        Task<int> OgrenciToplamDevamsizlikAsync(int ogrenciId);
    }
}
