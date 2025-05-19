using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;
using Serilog.Events;

using WarehouseApp.Infrastructure;
using WarehouseApp.UI;

namespace WarehouseApp
{
    public static class Program
    {
        public static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    path: "logs/warehouse-.log",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Information)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");

                var host = Host.CreateDefaultBuilder()
                    .UseSerilog()   // подключаем Serilog
                    .ConfigureServices((_, services) =>
                        services.AddWarehouseAppServices())
                    .Build();

                // Миграции
                var db = host.Services.GetRequiredService<WarehouseContext>();
                await db.Database.MigrateAsync();

                // Запуск UI
                var ui = host.Services.GetRequiredService<ConsoleUI>();
                await ui.RunAsync();

                Log.Information("Shutting down");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
