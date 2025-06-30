
namespace InventoryManager.Validators
{
    public class ValidationResult
    {
        public Dictionary<string, List<string>> Errors { get; }

        public bool IsValid => !Errors.Any();

        public ValidationResult(Dictionary<string, List<string>> errors)
        {
            Errors = errors ?? new Dictionary<string, List<string>>();
        }
    }
}