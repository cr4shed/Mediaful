using FluentValidation;
using Mediaful.Data.Models;

namespace Mediaful.Validation
{
    /// <summary>
    /// Class for validating Announcement Notifications.
    /// </summary>
    public class AnnouncementValidator : AbstractValidator<Notification>
    {
        /// <summary>
        /// Constructor which contains validation rules.
        /// </summary>
        public AnnouncementValidator()
        {
            RuleFor(a => a.Comment)
                .NotEmpty()
                .WithName("Announcement")

                .MaximumLength(512)
                .WithName("Announcement");
        }

        /// <summary>
        /// Method to validate the model using the above validation rules.
        /// </summary>
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<Notification>.CreateWithOptions((Notification)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
