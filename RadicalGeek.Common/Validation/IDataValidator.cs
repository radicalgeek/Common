using System.Collections.Generic;

namespace RadicalGeek.Common.Validation
{
    public interface IDataValidator<in T> where T : new()
    {
        ValidationResult Validate(IEnumerable<T> data);
    }
}
