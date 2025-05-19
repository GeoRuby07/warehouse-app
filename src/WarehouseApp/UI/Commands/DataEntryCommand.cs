using WarehouseApp.Application.Services;

namespace WarehouseApp.UI.Commands
{
    public class DataEntryCommand(IWarehouseService service) : IMenuCommand
    {
        public string Title => "Добавить данные";

        private readonly IWarehouseService _service = service;

        public void Execute() => DataEntryUI.DataEntryMenu(_service);
    }
}
