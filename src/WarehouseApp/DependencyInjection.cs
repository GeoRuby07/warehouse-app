using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WarehouseApp.Application.Repositories;
using WarehouseApp.Infrastructure;
using WarehouseApp.Infrastructure.Repositories;
using WarehouseApp.Services;
using WarehouseApp.UI;
using WarehouseApp.UI.Commands;

namespace WarehouseApp
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Регистрирует все сервисы, репозитории, контекст и UI-слой.
        /// </summary>
        public static IServiceCollection AddWarehouseAppServices(this IServiceCollection services)
        {
            // --- Infrastructure ---
            services.AddDbContext<WarehouseContext>(options =>
                options.UseSqlite("Data Source=warehouse.db"));
            services.AddScoped<IWarehouseContext>(sp =>
                sp.GetRequiredService<WarehouseContext>());

            // --- Repositories ---
            services.AddScoped<IBoxRepository, BoxRepository>();
            services.AddScoped<IPalletRepository, PalletRepository>();

            // --- Application services ---
            services.AddScoped<IWarehouseService, WarehouseService>();

            // --- UI commands & console UI ---
            services.AddScoped<IMenuCommand, ShowByExpirationCommand>();
            services.AddScoped<IMenuCommand, ShowTop3Command>();
            services.AddScoped<IMenuCommand, DataEntryCommand>();
            services.AddScoped<ConsoleUI>();

            return services;
        }
    }
}
