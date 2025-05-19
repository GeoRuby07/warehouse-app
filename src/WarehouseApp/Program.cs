using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WarehouseApp.Infrastructure;
using WarehouseApp.UI;

namespace WarehouseApp
{
    public static class Program
    {
        public static void Main()
        {
            // Собираем ServiceCollection и регистрируем ВСЁ одной строкой
            var services = new ServiceCollection()
                .AddWarehouseAppServices();

            var provider = services.BuildServiceProvider();

            // Применяем миграции
            var db = provider.GetRequiredService<WarehouseContext>();
            db.Database.Migrate();

            // Запускаем консольный интерфейс
            var ui = provider.GetRequiredService<ConsoleUI>();
            ui.Run();
        }
    }
}
