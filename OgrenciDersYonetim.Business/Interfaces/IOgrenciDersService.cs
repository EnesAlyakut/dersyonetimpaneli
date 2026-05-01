using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Business.Interfaces
{
    public interface IOgrenciDersService
    {
        Task<List<OgrenciDers>> OgrenciyeGoreDerslerGetirAsync(int ogrenciId);
        Task<List<OgrenciDers>> DerseGoreOgrencilerGetirAsync(int dersId);
        Task<bool> AtamaYapAsync(int ogrenciId, int dersId);
        Task<bool> AtamaKaldirAsync(int ogrenciId, int dersId);
        Task<bool> AtamaMevcutMuAsync(int ogrenciId, int dersId);
        Task<List<Ders>> OgrencininAtanmamisDersleriniGetirAsync(int ogrenciId);
    }
}
