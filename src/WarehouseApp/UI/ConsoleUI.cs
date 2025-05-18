using System;
using System.Linq;

using Spectre.Console;

using WarehouseApp.Services;

namespace WarehouseApp.UI {
    public static class ConsoleUI {
        public static void Run(WarehouseService service)
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Что вы хотите сделать?")
                        .AddChoices([
                            "1. Показать паллеты по сроку годности",
                            "2. Показать топ-3 паллеты",
                            "0. Выход"
                        ]));

                if (choice.StartsWith('0'))
                    break;

                switch (choice[0])
                {
                    case '1': ShowByExpiration(service); break;
                    case '2': ShowTop3(service); break;
                }

                AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу, чтобы вернуться в меню...[/]");
                Console.ReadKey(true);
                Console.Clear();
            }
        }

        private static void ShowByExpiration(WarehouseService svc)
        {
            AnsiConsole.MarkupLine("[underline]Паллеты по сроку годности[/]\n");

            foreach (var grp in svc.GroupByExpiration())
            {
                AnsiConsole.MarkupLine($"[green]Срок годности: {grp.Key:dd.MM.yyyy}[/]");

                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Вес");
                table.AddColumn("Объём");
                table.AddColumn("Кол-во коробок");

                foreach (var p in grp.OrderBy(p => p.Weight))
                {
                    table.AddRow(
                        p.Id.ToString(),
                        $"{p.Weight}kg",
                        $"{p.Volume}",
                        p.Boxes.Count.ToString());
                }

                AnsiConsole.Write(table);
                AnsiConsole.WriteLine();
            }
        }

        private static void ShowTop3(WarehouseService svc)
        {
            AnsiConsole.MarkupLine("[underline]Топ-3 паллеты по сроку годности[/]\n");

            var table = new Table();
            table.AddColumn("ID");
            table.AddColumn("Max Expiry");
            table.AddColumn("Вес");
            table.AddColumn("Объём");

            foreach (var p in svc.GetTop3ByMaxBoxExpiration())
            {
                var maxBox = p.Boxes.Max(b => b.ExpirationDate);
                table.AddRow(
                    p.Id.ToString(),
                    maxBox.ToString("dd.MM.yyyy"),
                    $"{p.Weight}kg",
                    $"{p.Volume}");
            }

            AnsiConsole.Write(table);
        }
    }
}
