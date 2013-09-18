using System.Collections.Generic;

namespace RadicalGeek.Common.Validation
{
    public interface IValidator<in T>
            where T : new()
    {
        List<ValidationResult> Validate(IEnumerable<T> records);
    }
}
