using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mvc.Data;
using Mvc.Models.Entity;
using Mvc.Service;
using Mvc.Service.Impl;

var builder = WebApplication.CreateBuilder(args);

// Agregue las configuraciones para la conexion
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL")));
builder.Services.AddIdentity<Usuario, IdentityRole>(cfg =>
    {
        cfg.User.RequireUniqueEmail = true;
        cfg.Password.RequireDigit = false;
        cfg.Password.RequiredUniqueChars = 0;
        cfg.Password.RequireLowercase = false;
        cfg.Password.RequireNonAlphanumeric = false;
        cfg.Password.RequireUppercase = false;

    }).AddEntityFrameworkStores<DatabaseContext>()
    .AddDefaultTokenProviders();
    // .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>();

builder.Services.AddNotyf(config =>
 {
     config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.TopCenter;
 });


builder.Services.AddTransient<SupervisorDb>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEnvioService, EnvioService>();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

SeedData(app);
void SeedData(WebApplication app)
{
    IServiceScopeFactory scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (IServiceScope scope = scopedFactory.CreateScope())
    {
        SupervisorDb service = scope.ServiceProvider.GetService<SupervisorDb>();
        service.SeedAsync().Wait();
    }
}


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
app.UseAuthentication(); //agregue la autenticacion
app.UseAuthorization();
app.UseNotyf(); //agregue notify
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=IniciarSesion}/{id?}");

app.Run();
