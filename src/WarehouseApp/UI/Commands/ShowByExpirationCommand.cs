using Microsoft.Extensions.Logging;

using WarehouseApp.Application.Services;

namespace WarehouseApp.UI.Commands
{
    public class ShowByExpirationCommand(IWarehouseService service, ILogger<ShowByExpirationCommand> logger) : IMenuCommand
    {
        public string Title => "Показать паллеты по сроку годности";

        private readonly IWarehouseService _service = service;
        private readonly ILogger<ShowByExpirationCommand> _logger = logger;

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Executing command: ShowByExpiration");
            await DisplayUI.ShowByExpirationAsync(_service);
        }
    }
}
