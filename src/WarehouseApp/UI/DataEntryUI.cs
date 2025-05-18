using Spectre.Console;

using WarehouseApp.Domain;
using WarehouseApp.Infrastructure;
using WarehouseApp.Services;

namespace WarehouseApp.UI {
    public static class DataEntryUI {
        public static void DataEntryMenu(WarehouseService service)
        {
            var ctx = service.GetDbContext();

            while (true)
            {
                var sub = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Добавление данных:")
                        .AddChoices("1. Создать коробку", "2. Создать паллету", "0. Назад"));

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
            var w = PromptDecimal("Ширина (см):");
            var h = PromptDecimal("Высота (см):");
            var d = PromptDecimal("Глубина (см):");
            var wt = PromptDecimal("Вес (кг):");

            DateTime? expInp = null, mfg = null;
            if (AnsiConsole.Confirm("Указать срок годности?"))
                expInp = AnsiConsole.Prompt(new TextPrompt<DateTime>("Срок (ГГГГ-MM-ДД):"));
            else
                mfg = AnsiConsole.Prompt(new TextPrompt<DateTime>("Дата пр-ва (ГГГГ-MM-ДД):"));

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
            AnsiConsole.MarkupLine($"[green]Коробка создана, Id: {box.Id}[/]");
        }

        private static void CreatePallet(WarehouseContext ctx)
        {
            AnsiConsole.MarkupLine("[underline]Создание паллеты[/]");
            var w = PromptDecimal("Ширина паллеты (см):");
            var h = PromptDecimal("Высота паллеты (см):");
            var d = PromptDecimal("Глубина паллеты (см):");

            var boxes = ctx.Boxes.ToList();
            if (boxes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]Нет коробок! Сначала создайте коробку[/]");
                return;
            }

            var select = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Box>()
                    .Title("Выберите коробки:")
                    .PageSize(10)
                    .AddChoices(boxes)
                    .UseConverter(b => $"{b.Id} ({b.Width}см x {b.Height}см x {b.Depth}см, срок:{b.ExpirationDate:yyyy-MM-dd})")
            );

            try
            {
                var pallet = new Pallet(w, h, d, select);
                ctx.Pallets.Add(pallet);
                ctx.SaveChanges();
                AnsiConsole.MarkupLine($"[green]Паллета создана, Id: {pallet.Id}[/]");
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Ошибка: {ex.Message}[/]");
            }
        }

        private static decimal PromptDecimal(string prompt) =>
            AnsiConsole.Prompt(
                new TextPrompt<decimal>(prompt)
                    .ValidationErrorMessage("<red>Введите число > 0</>")
                    .Validate(n => n > 0)
            );
    }
}
