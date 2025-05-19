using WarehouseApp.Application.Services;

namespace WarehouseApp.UI.Commands
{
    public class ShowByExpirationCommand(IWarehouseService service) : IMenuCommand
    {
        public string Title => "Показать паллеты по сроку годности";

        private readonly IWarehouseService _service = service;

        public async Task ExecuteAsync() => await DisplayUI.ShowByExpirationAsync(_service);
    }
}
