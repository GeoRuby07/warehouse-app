using Spectre.Console;

using WarehouseApp.UI.Commands;

namespace WarehouseApp.UI
{
    public class ConsoleUI(IEnumerable<IMenuCommand> commands)
    {
        private readonly IReadOnlyList<IMenuCommand> _commands = [.. commands];

        public void Run()
        {
            while (true)
            {
                // Собираем заголовки команд + опцию выхода
                var titles = new List<string>();
                for (int i = 0; i < _commands.Count; i++)
                    titles.Add($"{i + 1}. {_commands[i].Title}");
                titles.Add("0. Выход");

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Что вы хотите сделать?")
                        .AddChoices(titles));

                if (choice.StartsWith("0"))
                    break;

                // Парсим номер команды из "N. Title"
                var idx = int.Parse(choice.Split('.')[0]) - 1;
                _commands[idx].Execute();

                AnsiConsole.MarkupLine("\n[grey]Нажмите любую клавишу...[/]");
                Console.ReadKey(true);
                Console.Clear();
            }
        }
    }
}
