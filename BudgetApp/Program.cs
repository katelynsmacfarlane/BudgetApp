using BudgetApp.Models;
using Microsoft.EntityFrameworkCore;

IronPdf.License.LicenseKey = "IRONSUITE.KATELYNSMACFARLANE.GMAIL.COM." +
    "2971-99BAEB1585-HWF5ZFG4WO3VYJ-B6PTYKDDIZBN-WPKBQOWDUI3A-" +
    "B2NM5APLVCFW-GVZQCYAAAINW-CVJOMPENRSCT-ISHRSI-TO3A3RZXIGSMEA-DEPLOYMENT." +
    "TRIAL-BLUJF4.TRIAL.EXPIRES.29.MAR.2024";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BudgetAppContext>(option => option.UseSqlServer("server=(localdb)\\MSSQLLocalDB;database=BudgetApp;"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
