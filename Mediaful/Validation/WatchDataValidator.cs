using FluentValidation;
using Mediaful.Data.Models;

namespace Mediaful.Validation
{
    /// <summary>
    /// Class for validating WatchData.
    /// </summary>
    public class WatchDataValidator : AbstractValidator<WatchData>
    {
        /// <summary>
        /// Constructor which contains validation rules.
        /// </summary>
        public WatchDataValidator()
        {
            RuleFor(wd => wd.Title)
                .NotEmpty()
                .MaximumLength(128);
        }

        /// <summary>
        /// Method to validate the model using the above validation rules.
        /// </summary>
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<WatchData>.CreateWithOptions((WatchData)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
