using WarehouseApp.Application.Services;

namespace WarehouseApp.UI.Commands
{
    public class ShowTop3Command(IWarehouseService service) : IMenuCommand
    {
        public string Title => "Показать топ-3 паллеты";

        private readonly IWarehouseService _service = service;

        public async Task ExecuteAsync() => await DisplayUI.ShowTop3Async(_service);
    }
}
