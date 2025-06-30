
namespace InventoryManager.Services
{
    public interface IMessagingService
    {
        Task ShowMessageAsync(string title, string message, string buttonText = "OK");
        Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
    }
}
