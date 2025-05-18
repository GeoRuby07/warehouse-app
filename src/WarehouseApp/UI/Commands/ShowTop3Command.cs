using WarehouseApp.Services;

namespace WarehouseApp.UI.Commands
{
    public class ShowTop3Command(IWarehouseService service) : IMenuCommand
    {
        public string Title => "Показать топ-3 паллеты";

        private readonly IWarehouseService _service = service;

        public void Execute() => DisplayUI.ShowTop3(_service);
    }
}
