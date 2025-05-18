using Microsoft.EntityFrameworkCore;

using WarehouseApp.Infrastructure;
using WarehouseApp.Services;
using WarehouseApp.UI;

namespace WarehouseApp
{
    public static class Program
    {
        public static void Main()
        {
            using var db = new WarehouseContext();
            var service = new WarehouseService(db);
            db.Database.Migrate();
            ConsoleUI.Run(service);
        }
    }
}
