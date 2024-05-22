using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Validation;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace Mediaful.Pages.Admin
{
    /// <summary>
    /// Backend for the Feature Title page.
    /// </summary>
    public partial class FeatureTitle
    {
        /// <summary>
        /// MudForm reference.
        /// </summary>
        private MudForm form;

        /// <summary>
        /// Flag to check if the page has finished loading.
        /// </summary>
        private bool isLoading = false;

        /// <summary>
        /// FeatureInputFields object which simplifies input and input validation.
        /// </summary>
        public FeatureInputFields InputFields = new();

        /// <summary>
        /// List of previous featured titles which can be managed.
        /// </summary>
        private List<FeaturedTitle> features = new();

        /// <summary>
        /// The classes which are applied to the MudFileUpload component.
        /// Responsible for giving hover/drag feedback while uploading.
        /// </summary>
        private string DragClass = DefaultDragClass;

        /// <summary>
        /// Default classes for the MudFileUpload component.
        /// </summary>
        private static string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-4 mt-4 mud-width-full mud-height-full z-10";
        
        /// <summary>
        /// Validator object to validate the input fields.
        /// </summary>
        private FeatureInputFieldsValidator validator { get; set; } = new();

        /// <summary>
        /// Initializes the backend of the page.
        /// </summary>
        /// <returns>Task.</returns>
        protected override async Task OnInitializedAsync()
        {
            await UpdateFeatures();
        }

        /// <summary>
        /// Method which updates the list of previous features.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task UpdateFeatures()
        {
            features = await DatabaseService.GetRecords<FeaturedTitle>();
            StateHasChanged();
        }

        /// <summary>
        /// Method to handle the InputFileChange event when a user
        /// uploads a file to the MudFileUpload component.
        /// </summary>
        /// <param name="e">Event.</param>
        private void LoadFile(InputFileChangeEventArgs e)
        {
            InputFields.Image = e.File;
        }

        /// <summary>
        /// Method which clears the uploaded image.
        /// </summary>
        private void UnloadFile()
        {
            InputFields.Image = null;
            StateHasChanged();
        }

        /// <summary>
        /// Method to submit a new featured title.
        /// </summary>
        /// <returns>Task.</returns>
        private async Task Submit()
        {
            // Check if form input if valid.
            await form.Validate();
            if (!form.IsValid) return;

            // Create browser file object for the image.
            IBrowserFile file = InputFields.Image;

            // This should never return if the form validation is working.
            // Just a precautionary extra layer of protection.
            if (file == null || !file.ContentType.Contains("image")) return;

            try
            {
                // Create unique file name and path to store the image at.
                string date = DateTime.Now.ToString("ddMMyyyy_HHmmss_");
                var imgPath = Path.Combine("images", "uploads", date + file.Name);
                var fullPath = Path.Combine(Environment.ContentRootPath, "wwwroot", imgPath);

                // Write the file.
                await using FileStream writeStream = new(fullPath, FileMode.Create);
                await file.OpenReadStream().CopyToAsync(writeStream);

                // Create database object.
                FeaturedTitle feature = new()
                {
                    Description = InputFields.Description,
                    ImgPath = imgPath,
                    Expiry = DateTime.Now.AddMonths(1)
                };
                
                // Insert featured title into the database.
                await DatabaseService.Insert(feature);

                // Update list of previous featured titles.
                features.Add(feature);

                // Reset form.
                InputFields = new();
                await form.ResetAsync();

                StateHasChanged();
            }
            catch (Exception ex) { }

            isLoading = false;
        }

        /// <summary>
        /// Method to delete an existing featured title.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task Delete(int id)
        {
            // Show a dialog box to confirm the user wants to delete the featured title.
            bool? result = await DialogService.ShowMessageBox(
                "Delete",
                "Are you sure you want to delete this feature?",
                yesText: "Delete", cancelText: "Cancel");

            // If user confirms deletion.
            if (result != null)
            {
                // Delete the featured title.
                await DatabaseService.Delete<FeaturedTitle>(id);
                await UpdateFeatures();

                // Use snackbar to display success message.
                Snackbar.Clear();
                Snackbar.Add($"Successfully deleted", Severity.Success);
            }

            StateHasChanged();
        }

        /// <summary>
        /// Helper method to update the MudFileUpload classes.
        /// </summary>
        private void SetDragClass()
        {
            DragClass = $"{DefaultDragClass} mud-border-primary";
        }

        /// <summary>
        /// Helper method to reset the MudFileUpload classes.
        /// </summary>
        private void ClearDragClass()
        {
            DragClass = DefaultDragClass;
        }
    }

    /// <summary>
    /// Class to simplify input and input validation.
    /// Used only within the FeatureTitle page.
    /// </summary>
    public class FeatureInputFields
    {
        /// <summary>
        /// Description of the feature.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Uploaded image.
        /// </summary>
        public IBrowserFile Image { get; set; }
    }
}
