using Spectre.Console;

using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;
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
                            "3. Добавить данные",
                            "0. Выход"
                        ]));

                if (choice.StartsWith('0'))
                    break;

                switch (choice[0])
                {
                    case '1': ShowByExpiration(service); break;
                    case '2': ShowTop3(service); break;
                    case '3': DataEntryMenu(service); break;
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

        private static void DataEntryMenu(WarehouseService service)
        {
            var ctx = service.GetDbContext();

            while (true)
            {
                var sub = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Добавление данных:")
                        .AddChoices([
                        "1. Создать коробку",
                        "2. Создать паллету",
                        "0. Назад"
                        ]));

                if (sub.StartsWith('0')) break;

                switch (sub[0])
                {
                    case '1': CreateBox(ctx); break;
                    case '2': CreatePallet(ctx); break;
                }
            }
        }

        private static void CreateBox(WarehouseContext ctx)
        {
            AnsiConsole.MarkupLine("[underline]Создание коробки[/]");
            var w = AnsiConsole.Prompt(new TextPrompt<decimal>("Width?"));
            var h = AnsiConsole.Prompt(new TextPrompt<decimal>("Height?"));
            var d = AnsiConsole.Prompt(new TextPrompt<decimal>("Depth?"));
            var wt = AnsiConsole.Prompt(new TextPrompt<decimal>("Weight?"));

            // Спрашиваем, вводить ли срок годности
            DateTime? expInp = null, mfg = null;
            if (AnsiConsole.Confirm("Указать срок годности?"))
                expInp = AnsiConsole.Prompt(new TextPrompt<DateTime>("Expiration date (YYYY-MM-DD):"));
            else
                mfg = AnsiConsole.Prompt(new TextPrompt<DateTime>("Manufacture date (YYYY-MM-DD):"));

            var box = new Box
            {
                Width = w,
                Height = h,
                Depth = d,
                Weight = wt,
                ExpirationDateInput = expInp,
                ManufactureDate = mfg
            };

            ctx.Boxes.Add(box);
            ctx.SaveChanges();
            AnsiConsole.MarkupLine($"[green]Box created with Id: {box.Id}[/]");
        }

        private static void CreatePallet(WarehouseContext ctx)
        {
            AnsiConsole.MarkupLine("[underline]Создание паллеты[/]");
            var w = AnsiConsole.Prompt(new TextPrompt<decimal>("Width?"));
            var h = AnsiConsole.Prompt(new TextPrompt<decimal>("Height?"));
            var d = AnsiConsole.Prompt(new TextPrompt<decimal>("Depth?"));

            // Выбор уже существующих коробок
            var allBoxes = ctx.Boxes.ToList();
            if (allBoxes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Нет сохранённых коробок! Сначала создайте их.[/]");
                return;
            }

            var selection = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Box>()
                    .Title("Выберите коробки для паллеты:")
                    .NotRequired()
                    .PageSize(10)
                    .AddChoices(allBoxes)
                    .UseConverter(box => $"{box.Id} ({box.Width}×{box.Depth}, exp:{box.ExpirationDate:yyyy-MM-dd})")
            );

            try
            {
                var pallet = new Pallet(w, h, d, selection);
                ctx.Pallets.Add(pallet);
                ctx.SaveChanges();
                AnsiConsole.MarkupLine($"[green]Pallet created with Id: {pallet.Id}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            }
        }

    }
}
