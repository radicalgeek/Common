using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RadicalGeek.Common.Validation
{
    public class Validator<T> : IValidator<T> where T : new()
    {
        private readonly List<IDataValidator<T>> dataValidators = new List<IDataValidator<T>>();

        public Validator()
        {
            IEnumerable<Type> types = typeof (T).Assembly.GetTypes().Union(Assembly.GetCallingAssembly().GetTypes()).Union(Assembly.GetExecutingAssembly().GetTypes()).Distinct();
            IEnumerable<Type> myDataValidators = types.Where(t => t.GetInterfaces().Contains(typeof(IDataValidator<T>)));
            foreach (Type validatorType in myDataValidators)
                dataValidators.Add((IDataValidator<T>) Activator.CreateInstance(validatorType));
        }

        public List<ValidationResult> Validate(IEnumerable<T> records)
        {
            return dataValidators.Select(validator => validator.Validate(records)).ToList();
        }
    }
}
