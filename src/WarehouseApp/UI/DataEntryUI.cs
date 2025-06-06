using Spectre.Console;

using WarehouseApp.Application.Services;
using WarehouseApp.Domain;

namespace WarehouseApp.UI
{
    public static class DataEntryUI
    {
        public static async Task DataEntryMenu(IWarehouseService service)
        {
            while (true)
            {
                var sub = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Добавление данных:")
                        .AddChoices("1. Создать коробку", "2. Создать паллету", "0. Назад"));

                if (sub.StartsWith("0"))
                    break;

                if (sub.StartsWith("1"))
                    await CreateBoxAsync(service);
                else
                    await CreatePalletAsync(service);
            }
        }

        private static async Task CreateBoxAsync(IWarehouseService service)
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

            box = await service.CreateBoxAsync(box);
            AnsiConsole.MarkupLine($"[green]Коробка создана, Id: {box.Id}[/]");
        }

        private static async Task CreatePalletAsync(IWarehouseService service)
        {
            AnsiConsole.MarkupLine("[underline]Создание паллеты[/]");

            var w = PromptDecimal("Ширина паллеты (см):");
            var h = PromptDecimal("Высота паллеты (см):");
            var d = PromptDecimal("Глубина паллеты (см):");

            var available = (await service.GetAvailableBoxesAsync()).ToList();
            if (!available.Any())
            {
                AnsiConsole.MarkupLine("[red]Нет свободных коробок для паллеты![/]");
                return;
            }

            var selectedIds = AnsiConsole.Prompt(
                new MultiSelectionPrompt<Guid>()
                    .Title("Выберите коробки для паллеты:")
                    .PageSize(10)
                    .AddChoices(available.Select(b => b.Id))
                    .UseConverter(id =>
                    {
                        var b = available.First(x => x.Id == id);
                        return $"{b.Id} ({b.Width}cm x {b.Height}cm x {b.Depth}cm, exp:{b.ExpirationDate:yyyy-MM-dd})";
                    }
                    )
            );

            if (!selectedIds.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Ни одна коробка не выбрана — отмена.[/]");
                return;
            }

            var pallet = service.CreatePalletAsync(w, h, d, selectedIds);
            AnsiConsole.MarkupLine($"[green]Паллета создана, Id: {pallet.Id}[/]");
        }

        private static decimal PromptDecimal(string prompt) =>
            AnsiConsole.Prompt(
                new TextPrompt<decimal>(prompt)
                    .ValidationErrorMessage("[red]Введите число[/]")
                    .Validate(n => n > 0)
            );
    }
}
