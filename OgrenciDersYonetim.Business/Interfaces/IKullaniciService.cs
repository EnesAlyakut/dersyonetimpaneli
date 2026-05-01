using OgrenciDersYonetim.Core.Entities;

namespace OgrenciDersYonetim.Business.Interfaces
{
    public interface IKullaniciService
    {
        Task<Kullanici?> GirisYapAsync(string kullaniciAdi, string sifre);
        Task<bool> KayitOlAsync(Kullanici kullanici, string sifre);
        Task<Kullanici?> IdIleGetirAsync(int id);
        Task<List<Kullanici>> TumunuGetirAsync();
        Task<bool> KullaniciAdiMevcutMuAsync(string kullaniciAdi);
        Task<bool> EmailMevcutMuAsync(string email);
        Task GuncelleAsync(Kullanici kullanici);
        Task SifreDegistirAsync(int kullaniciId, string yeniSifre);
        string SifreyiHashle(string sifre);
    }
}
