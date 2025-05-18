namespace WarehouseApp.UI.Commands
{
    public interface IMenuCommand
    {
        string Title { get; }
        void Execute();
    }
}
