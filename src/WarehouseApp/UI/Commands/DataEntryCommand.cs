using Microsoft.Extensions.Logging;

using WarehouseApp.Application.Services;

namespace WarehouseApp.UI.Commands
{
    public class DataEntryCommand(IWarehouseService service, ILogger<ShowByExpirationCommand> logger) : IMenuCommand
    {
        public string Title => "Добавить данные";

        private readonly IWarehouseService _service = service;
        private readonly ILogger<ShowByExpirationCommand> _logger = logger;

        public async Task ExecuteAsync()
        {
            _logger.LogInformation("Executing command: DataEntryMenu");
            await DataEntryUI.DataEntryMenu(_service);
        }
    }
}
