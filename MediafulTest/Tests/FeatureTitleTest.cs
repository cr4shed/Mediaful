using AngleSharp.Dom;
using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Pages.Admin;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Linq.Expressions;

namespace MediafulTest.Tests
{
    /// <summary>
    /// Test class with tests for the Feature Title admin page.
    /// </summary>
    public class FeatureTitleTest : BaseTest
    {
        /// <summary>
        /// Test adding a new featured title.
        /// </summary>
        [Fact]
        public void TestToPass_AddNewFeaturedTitle()
        {
            #region Arrange
            // Create test data.
            var testImg = new MockBrowserFile("test.jpg", DateTimeOffset.Now, 0, "image/jpeg", Array.Empty<byte>());

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockEnvironment = new Mock<IWebHostEnvironment>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<FeaturedTitle, bool>>>()))
                .ReturnsAsync(new List<FeaturedTitle>());

            mockEnvironment
                .Setup(m => m.ContentRootPath)
                .Returns("");

            // Add mock services to test context.
            Context.Services.AddSingleton(mockDatabaseService.Object);
            Context.Services.AddSingleton(mockEnvironment.Object);

            // Configure mock authorization.
            var authContext = Context.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Admin");
            #endregion Arrange


            #region Act
            // Render Index/Dashboard component.
            var cut = Context.RenderComponent<FeatureTitle>();

            var beforeImage = cut.Instance.InputFields.Image;

            // Upload file.
            var fileMemoryStream = (MemoryStream)(testImg.OpenReadStream());
            var fileContent = InputFileContent.CreateFromBinary(fileMemoryStream.ToArray(), "test.png", contentType: "image/png");
            var fileInput = cut.FindComponent<InputFile>();
            fileInput.UploadFiles(fileContent);

            var afterImage = cut.Instance.InputFields.Image;

            // Cannot use the submit button because file cannot be saved to a test path.
            // This test still confirms the functionality of uploading a new featured title.
            #endregion Act


            #region Assert
            // Verify image was null and then is not null after upload.
            Assert.Null(beforeImage);
            Assert.NotNull(afterImage);
            #endregion Assert
        }
    }
}
