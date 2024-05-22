using FluentValidation;
using Mediaful.Pages.Admin;

namespace Mediaful.Validation
{
    /// <summary>
    /// Class for validating the Featured Title input fields.
    /// </summary>
    public class FeatureInputFieldsValidator : AbstractValidator<FeatureInputFields>
    {
        /// <summary>
        /// Constructor which contains validation rules.
        /// </summary>
        public FeatureInputFieldsValidator()
        {
            RuleFor(f => f.Description)
                .Cascade(CascadeMode.Stop)

                .NotEmpty()
                .WithMessage("Description is required.");

            RuleFor(f => f.Image)
                .Cascade(CascadeMode.Stop)

                .NotEmpty()
                .WithMessage("An image upload is required.")

                .Must(f => f.ContentType.Contains("image"))
                .WithMessage("Uploaded file must be an image.");

                When(x => x.Image != null, () =>
                {
                    RuleFor(x => x.Image.Size).LessThanOrEqualTo(10485760).WithMessage("The maximum image size is 10 MB");
                });
        }

        /// <summary>
        /// Method to validate the model using the above validation rules.
        /// </summary>
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<FeatureInputFields>.CreateWithOptions((FeatureInputFields)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
