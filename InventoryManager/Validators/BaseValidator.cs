using System.Text.RegularExpressions;

namespace InventoryManager.Validators
{
    public abstract class BaseValidator<T>
    {
        protected readonly IEnumerable<T> _items;

        protected BaseValidator(IEnumerable<T> items)
        {
            _items = items;
        }

        public abstract Dictionary<string, string> Rules();
        public abstract Dictionary<string, string> Messages();

        public ValidationResult Validate(T model)
        {
            var errors = new Dictionary<string, List<string>>();
            var rules = Rules();
            var messages = Messages();

            foreach (var kvp in rules)
            {
                var field = kvp.Key;
                var parts = kvp.Value.Split('|');
                var prop = typeof(T).GetProperty(field);
                var rawValue = prop != null ? prop.GetValue(model) : null;
                var value = rawValue?.ToString();

                foreach (var rulePart in parts)
                {
                    var seg = rulePart.Split(new[] { ':' }, 2);
                    var name = seg[0];
                    var param = seg.Length > 1 ? seg[1] : null;

                    if (!ApplyRule(name, param ?? string.Empty, value ?? string.Empty, field))
                    {
                        var key = $"{field}.{name}";
                        var msg = messages.ContainsKey(key)
                            ? messages[key]
                            : $"El campo {field} no cumple la regla {name}.";
                        if (!errors.ContainsKey(field)) errors[field] = new List<string>();
                        errors[field].Add(msg);
                    }
                }
            }

            return new ValidationResult(errors);
        }

        private bool ApplyRule(string rule, string param, string value, string field)
        {
            switch (rule)
            {
                case "required":
                    return !string.IsNullOrWhiteSpace(value);
                case "nullable":
                    return true; // Permite nulos, no valida nada
                case "min":
                    if (float.TryParse(value, out var numMin))
                        return numMin >= float.Parse(param);
                    return value?.Length >= int.Parse(param);
                case "max":
                    if (float.TryParse(value, out var numMax))
                        return numMax <= float.Parse(param);
                    return value?.Length <= int.Parse(param);
                case "length":
                    return value?.Length == int.Parse(param);
                case "regex":
                    return Regex.IsMatch(value ?? "", param);
                case "email":
                    return Regex.IsMatch(value ?? "", @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                case "digits":
                    return Regex.IsMatch(value ?? "", @"^\d+$");
                case "between":
                    var nums = param.Split(',');
                    if (float.TryParse(value, out var numBetween))
                    {
                        float minNum = float.Parse(nums[0]), maxNum = float.Parse(nums[1]);
                        return numBetween >= minNum && numBetween <= maxNum;
                    }
                    int min = int.Parse(nums[0]), max = int.Parse(nums[1]);
                    return value?.Length >= min && value.Length <= max;
                case "unique":
                    var p = typeof(T).GetProperty(param);
                    if (p == null) return true;
                    return !_items.Any(x =>
                        string.Equals(p.GetValue(x)?.ToString() ?? string.Empty, value ?? string.Empty, StringComparison.OrdinalIgnoreCase));
                case "integer":
                    return int.TryParse(value, out _);
                case "float":
                    return float.TryParse(value, out _);
                case "date":
                    return DateTime.TryParse(value, out _);
                case "datetime":
                    return DateTime.TryParse(value, out _);
                case "in":
                    var allowed = param.Split(',');
                    return allowed.Contains(value);
                case "not_in":
                    var notAllowed = param.Split(',');
                    return !notAllowed.Contains(value);
                case "confirmed":
                    var confirmProp = typeof(T).GetProperty(field + "Confirmation");
                    var confirmValue = confirmProp?.GetValue(_items.FirstOrDefault())?.ToString();
                    return value == confirmValue;
                case "starts_with":
                    return value != null && value.StartsWith(param);
                case "ends_with":
                    return value != null && value.EndsWith(param);
                case "before":
                    if (DateTime.TryParse(value, out var dateBefore) && DateTime.TryParse(param, out var dateParamBefore))
                        return dateBefore < dateParamBefore;
                    return false;
                case "after":
                    if (DateTime.TryParse(value, out var dateAfter) && DateTime.TryParse(param, out var dateParamAfter))
                        return dateAfter > dateParamAfter;
                    return false;
                case "boolean":
                    return value == "true" || value == "false";
                case "url":
                    return Regex.IsMatch(value ?? "", @"^(https?|ftp)://[^\s/$.?#].[^\s]*$");
                case "alpha":
                    return Regex.IsMatch(value ?? "", @"^[a-zA-Z]+$");
                case "alpha_num":
                    return Regex.IsMatch(value ?? "", @"^[a-zA-Z0-9]+$");
                case "same":
                    var sameProp = typeof(T).GetProperty(param);
                    var sameValue = sameProp?.GetValue(_items.FirstOrDefault())?.ToString();
                    return value == sameValue;
                default:
                    return true;
            }
        }
    }
}