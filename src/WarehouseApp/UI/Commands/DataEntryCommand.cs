using WarehouseApp.Application.Services;

namespace WarehouseApp.UI.Commands
{
    public class DataEntryCommand(IWarehouseService service) : IMenuCommand
    {
        public string Title => "Добавить данные";

        private readonly IWarehouseService _service = service;

        public async Task ExecuteAsync() => await DataEntryUI.DataEntryMenu(_service);
    }
}
