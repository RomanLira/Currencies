using System.Text;
using Currency.Core.Dto.Mappings;
using Currency.Core.Interfaces;
using Currency.Infrastructure;
using Currency.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("appsettings.json", optional: false);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
        
        services.AddHttpClient<ICbrClient, CbrClient>();
        
        services.AddScoped<IExchangeRateService, ExchangeRateService>();

        services.AddAutoMapper(con => con.AddProfile(new AssemblyMappingProfile()));
    })
    .Build();

using var scope = host.Services.CreateScope();
var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
var service = scope.ServiceProvider.GetRequiredService<IExchangeRateService>();

int days = config.GetValue<int>("Cbr:DaysToLoad");

Console.WriteLine($"Загрузка курсов за последние {days} дней...");

await service.ImportRangeAsync(days);

Console.WriteLine("Готово!");

Console.ReadLine();