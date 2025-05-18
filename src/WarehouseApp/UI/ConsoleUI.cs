using Spectre.Console;

using WarehouseApp.Services;

namespace WarehouseApp.UI {
    public static class ConsoleUI {
        private static readonly string[] MainOptions = [
            "1. Показать паллеты по сроку годности",
            "2. Показать топ-3 паллеты",
            "3. Добавить данные",
            "0. Выход"
        ];

        public static void Run(WarehouseService service)
        {
            while (true)
            {
                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Что вы хотите сделать?")
                        .AddChoices(MainOptions));

                if (choice.StartsWith('0')) break;

                switch (choice[0])
                {
                    case '1': DisplayUI.ShowByExpiration(service); break;
                    case '2': DisplayUI.ShowTop3(service); break;
                    case '3': DataEntryUI.DataEntryMenu(service); break;
                }

                AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу...[/]");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}
