using Spectre.Console;

using WarehouseApp.Application.Services;

namespace WarehouseApp.UI
{
    public static class DisplayUI
    {
        public static async Task ShowByExpirationAsync(IWarehouseService svc)
        {
            AnsiConsole.MarkupLine("[underline]Паллеты по сроку годности[/]\n");
            var grouped = await svc.GroupByExpirationAsync();
            foreach (var grp in grouped)
            {
                AnsiConsole.MarkupLine($"[green]Срок: {grp.Key:dd.MM.yyyy}[/]");

                var table = new Table()
                    .AddColumn("ID")
                    .AddColumn("Вес (кг)")
                    .AddColumn("Объем (куб.см)")
                    .AddColumn("Кол-во коробок");

                foreach (var p in grp.OrderBy(p => p.Weight))
                {
                    table.AddRow(p.Id.ToString(),
                                 $"{p.Weight} кг",
                                 $"{p.Volume} куб.см",
                                 p.Boxes.Count.ToString());
                }

                AnsiConsole.Write(table);
                Console.WriteLine();
            }
        }

        public static async Task ShowTop3Async(IWarehouseService svc)
        {
            AnsiConsole.MarkupLine("[underline]Топ-3 паллеты[/]\n");

            var table = new Table()
                .AddColumn("ID")
                .AddColumn("Срок")
                .AddColumn("Вес (кг)")
                .AddColumn("Объем (куб.см)");

            foreach (var p in await svc.GetTop3ByMaxBoxExpirationAsync())
            {
                var maxBox = p.Boxes.Max(b => b.ExpirationDate);
                table.AddRow(p.Id.ToString(),
                             maxBox.ToString("dd.MM.yyyy"),
                             $"{p.Weight} кг",
                             $"{p.Volume} куб.см");
            }

            AnsiConsole.Write(table);
        }
    }
}
