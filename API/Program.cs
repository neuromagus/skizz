using System.Security.Cryptography.X509Certificates;
using API.Middleware;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<StoreContext>(opt => 
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddCors();
builder.Services.AddSingleton<IConnectionMultiplexer>(config => {
    var connectionString = builder.Configuration.GetConnectionString("Redis") 
        ?? throw new Exception("Cannot get Redis connection string");
    var configuration = ConfigurationOptions.Parse(connectionString, true);

    return ConnectionMultiplexer.Connect(configuration);
});
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<AppUser>().AddEntityFrameworkStores<StoreContext>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5051, listenOptions =>
    {
        listenOptions.UseHttps(httpsOptions =>
        {
            var cert = new X509Certificate2("ssl/localhost.pfx", "");  // Use empty string if no password
            httpsOptions.ServerCertificate = cert;
        });
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x.AllowAnyHeader()
   .AllowAnyMethod()
   .AllowCredentials()
   .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.MapControllers();

// now /login and other Identities endpoints moved to /api/login;
// this way removed any problems with consistent endpoints from api and frondend
app.MapGroup("api").MapIdentityApi<AppUser>();

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}

app.Run();
