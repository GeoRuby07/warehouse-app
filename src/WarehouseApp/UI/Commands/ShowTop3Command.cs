using Microsoft.Extensions.Logging;

using WarehouseApp.Application.Services;

namespace WarehouseApp.UI.Commands
{
    public class ShowTop3Command(IWarehouseService service, ILogger<ShowByExpirationCommand> logger) : IMenuCommand
    {
        public string Title => "Показать топ-3 паллеты";

        private readonly IWarehouseService _service = service;
        private readonly ILogger<ShowByExpirationCommand> _logger = logger;

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Executing command: ShowTop3Async");
            await DisplayUI.ShowTop3Async(_service);
        }
    }
}
