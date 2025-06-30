namespace InventoryManager.Services
{
    public class MessagingService : IMessagingService
    {
        public Task ShowMessageAsync(string title, string message, string buttonText = "OK")
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                return mainPage.DisplayAlert(title, message, buttonText);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
        {
            var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                return mainPage.DisplayAlert(title, message, accept, cancel);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}