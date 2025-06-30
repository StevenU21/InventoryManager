namespace InventoryManager.Services
{
    public interface IValidationService<T>
    {
        Task<(bool IsValid, string? ErrorMessage)> ValidateAsync(T entity, bool isEdit = false);
    }
}