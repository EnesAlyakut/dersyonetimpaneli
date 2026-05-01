using Microsoft.EntityFrameworkCore;
using OgrenciDersYonetim.Business.Interfaces;
using OgrenciDersYonetim.Business.Services;
using OgrenciDersYonetim.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// MVC ve Razor View Engine
builder.Services.AddControllersWithViews();

// SQLite veritabanı bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session yapılandırması
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".OgrenciYonetim.Session";
});

builder.Services.AddHttpContextAccessor();

// Business katmanı servislerinin DI kaydı
builder.Services.AddScoped<IKullaniciService, KullaniciService>();
builder.Services.AddScoped<IOgrenciService, OgrenciService>();
builder.Services.AddScoped<IDersService, DersService>();
builder.Services.AddScoped<IOgrenciDersService, OgrenciDersService>();
builder.Services.AddScoped<IOgretmenService, OgretmenService>();
builder.Services.AddScoped<INotService, NotService>();
builder.Services.AddScoped<IDevamsizlikService, DevamsizlikService>();
builder.Services.AddScoped<IDersIcerikService, DersIcerikService>();

var app = builder.Build();

// Veritabanı otomatik migration ve seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session middleware - Authorization'dan ÖNCE olmalı
app.UseSession();

app.UseAuthorization();

// Route tanımlaması
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
