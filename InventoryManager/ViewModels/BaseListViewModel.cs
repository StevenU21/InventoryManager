using System.Collections.ObjectModel;
using System.Windows.Input;
using InventoryManager.Models;

namespace InventoryManager.ViewModels
{
    public abstract class BaseListViewModel<T> : ObservableObject
    {
        public ObservableCollection<T> Items { get; } = new();
        public bool IsBusy { get; set; }
        public ICommand LoadItemsCommand { get; }

        protected BaseListViewModel()
        {
            LoadItemsCommand = new Command(async () => await LoadItemsAsync());
        }

        protected abstract Task<IEnumerable<T>> GetItemsAsync();

        public async Task LoadItemsAsync()
        {
            IsBusy = true;
            var items = await GetItemsAsync();
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
            IsBusy = false;
        }
    }
}