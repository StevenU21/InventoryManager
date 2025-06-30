using InventoryManager.Validators;

namespace InventoryManager.Services
{
    public class ValidationService<T> : IValidationService<T> where T : class
    {
        private readonly IBaseService<T> _service;
        private readonly BaseValidator<T> _validator;

        public ValidationService(IBaseService<T> service, BaseValidator<T> validator)
        {
            _service = service;
            _validator = validator;
        }

        public async Task<(bool IsValid, string? ErrorMessage)> ValidateAsync(T entity, bool isEdit = false)
        {
            var allEntities = await _service.GetAllAsync();
            var entitiesToValidate = allEntities;

            if (isEdit)
            {
                var idProp = typeof(T).GetProperty("Id");
                if (idProp != null)
                {
                    var entityId = idProp.GetValue(entity);
                    entitiesToValidate = allEntities.Where(e => idProp.GetValue(e)?.Equals(entityId) == false).ToList();
                }
            }

            var validator = (BaseValidator<T>)Activator.CreateInstance(_validator.GetType(), entitiesToValidate)!;
            var result = validator.Validate(entity);

            if (!result.IsValid)
            {
                var errors = result.Errors.SelectMany(kvp => kvp.Value).ToArray();
                var errorMessage = string.Join(" ", errors);
                return (false, errorMessage);
            }

            return (true, null);
        }
    }
}