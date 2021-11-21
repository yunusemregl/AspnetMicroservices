using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ordering.Application.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public ValidationException() : base("One or more validation faliures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }


        public ValidationException(IEnumerable<ValidationFailure> failures)
        {
            Errors = failures.GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }


        public IDictionary<string, string[]> Errors { get; }
    }
}
