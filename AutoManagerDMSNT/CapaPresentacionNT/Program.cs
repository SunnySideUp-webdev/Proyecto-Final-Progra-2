
using CapaAccesoDatosNT;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar Razor Pages
builder.Services.AddRazorPages();

// Configurar SQL Server
builder.Services.AddDbContext<clsAutoManagerContextNT>(
    pObjOpcionesNT =>
        pObjOpcionesNT.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "AutoManagerDMS")
        )
);

var app = builder.Build();

// Configurar la aplicación web
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
