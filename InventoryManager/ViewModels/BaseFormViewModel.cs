using System.Windows.Input;
using InventoryManager.Services;
using InventoryManager.Models;

namespace InventoryManager.ViewModels
{
    public abstract class BaseFormViewModel<T> : ObservableObject
    {
        protected readonly IMessagingService _messagingService;
        protected readonly IValidationService<T> _validationService;
        protected readonly INavigation _navigation;

        public T? Entity { get; set; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public bool IsBusy { get; set; }
        public event Action<T>? EntitySaved;

        protected BaseFormViewModel(
            IValidationService<T> validationService,
            IMessagingService messagingService,
            INavigation navigation)
        {
            _validationService = validationService;
            _messagingService = messagingService;
            _navigation = navigation;
            SaveCommand = new Command(async () => await SaveAsync());
            CancelCommand = new Command(async () => await _navigation.PopModalAsync());
        }

        protected virtual async Task SaveAsync()
        {
            if (Entity == null)
            {
                await _messagingService.ShowMessageAsync("Error", "Entity cannot be null.");
                return;
            }

                var (isValid, errorMessage) = await _validationService.ValidateAsync(Entity, GetIsEdit());
            if (!isValid)
            {
                await _messagingService.ShowMessageAsync("Error", errorMessage ?? "Unknown error");
                return;
            }
            await SaveOrUpdateAsync();
            EntitySaved?.Invoke(Entity);
            await _navigation.PopModalAsync();
        }

        protected abstract Task SaveOrUpdateAsync();
        protected abstract bool GetIsEdit();
    }
}