using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Validation;

namespace RadicalGeek.Common.UnitTests
{
    /// <summary>
    /// Summary description for ValidatorTests
    /// </summary>
    [TestClass]
    public class ValidatorTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void ValidateShouldReturnTwoValidationResults()
        {
            List<MyClass> data = new List<MyClass>
                {
                new MyClass { Message = "Hello", Count = 100 },
                new MyClass { Message = "InvalidMessage", Count = 75},
                new MyClass { Message = "InvalidMessage", Count = 1},
            };

            Validator<MyClass> validator = new Validator<MyClass>();

            List<ValidationResult> results = validator.Validate(data);

            Assert.AreEqual(2, results.Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ValidateShouldReturnTwoErrorsForTheCountValidator()
        {
            List<MyClass> data = new List<MyClass>
                {
                new MyClass { Message = "Hello", Count = 7 },
                new MyClass { Message = "InvalidMessage", Count = 100},
                new MyClass { Message = "InvalidMessage", Count = 1},
            };

            Validator<MyClass> validator = new Validator<MyClass>();

            List<ValidationResult> results = validator.Validate(data);

            Assert.AreEqual(2, results.Find(x => x.ResultName == "Count").Count);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ValidateShouldReturnOneErrorForTheMessageValidator()
        {
            List<MyClass> data = new List<MyClass>
                {
                new MyClass { Message = "Hello", Count = 100 },
                new MyClass { Message = "Message", Count = 75},
                new MyClass { Message = "InvalidMessage", Count = 1},
            };

            Validator<MyClass> validator = new Validator<MyClass>();

            List<ValidationResult> results = validator.Validate(data);

            Assert.AreEqual(1, results.Find(x => x.ResultName == "Count").Count);
        }
    }

    public class MyClass
    {
        public int Count { get; set; }
        public string Message { get; set; }
    }

    public class CountValidator : IDataValidator<MyClass>
    {
        public ValidationResult Validate(IEnumerable<MyClass> data)
        {
            return new ValidationResult { ResultName = "Count", Count = data.Count(x => x.Count < 10) };
        }
    }

    public class MessageValidator : IDataValidator<MyClass>
    {
        public ValidationResult Validate(IEnumerable<MyClass> data)
        {
            return new ValidationResult { ResultName = "Message", Count = data.Count(x => x.Message.Contains("InvalidMessage")) };
        }
    }
}
