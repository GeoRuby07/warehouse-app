using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WarehouseApp.Infrastructure;
using WarehouseApp.Services;
using WarehouseApp.UI;
using WarehouseApp.UI.Commands;

namespace WarehouseApp
{
    public static class Program
    {
        public static void Main()
        {
            // Собираем ServiceCollection
            var services = new ServiceCollection();

            // Регистрируем DbContext и абстракции
            services.AddDbContext<WarehouseContext>(options =>
                options.UseSqlite("Data Source=warehouse.db"));
            services.AddScoped<IWarehouseContext>(sp => sp.GetRequiredService<WarehouseContext>());

            // Регистрируем сервис приложения
            services.AddScoped<IWarehouseService, WarehouseService>();

            // Регистрируем UI
            services.AddScoped<IMenuCommand, ShowByExpirationCommand>();
            services.AddScoped<IMenuCommand, ShowTop3Command>();
            services.AddScoped<IMenuCommand, DataEntryCommand>();
            services.AddScoped<ConsoleUI>();

            // Строим провайдер
            var provider = services.BuildServiceProvider();

            // Применяем миграции
            var db = provider.GetRequiredService<WarehouseContext>();
            db.Database.Migrate();

            // Запускаем UI, резолвим ConsoleUI
            var ui = provider.GetRequiredService<ConsoleUI>();
            ui.Run();
        }
    }
}
