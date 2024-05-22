using AngleSharp.Dom;
using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Pages.User;
using System.Linq.Expressions;
using System.Text;

namespace MediafulTest.Tests
{
    /// <summary>
    /// Test class with test cases for the Log Title user page.
    /// </summary>
    public class LogTitleTest : BaseTest
    {
        /// <summary>
        /// Test case to test an authorized user adding a new watched item to their collection.
        /// </summary>
        [Fact]
        public void TestToPass_AddNewWatchedItem()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestUser", IsBanned = false };

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            // Add mock services to test context.
            Context.Services.AddSingleton(mockDatabaseService.Object);
            Context.Services.AddSingleton(mockAuthService.Object);

            // Configure mock authorization.
            var authContext = Context.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Admin");
            #endregion Arrange


            #region Act
            // Render Index/Dashboard component.
            var cut = Context.RenderComponent<LogTitle>();

            var titleInput = cut.Find(".mud-autocomplete input");
            titleInput.SetInnerText("Lost");
            titleInput.Input("Lost");

            var submitButton = cut.Find("button.mud-button-filled-primary");
            submitButton.Click();
            #endregion Act


            #region Assert
            // Submitting the form will reset the input. Will fail if submission or validation fails.
            Assert.Equal(string.Empty, titleInput.TextContent);
            #endregion Assert
        }

        /// <summary>
        /// Test case to test an authorized user attempting to submit a new watched item with an empty title.
        /// </summary>
        [Fact]
        public void TestToFail_AddNewWatchedItemWithEmptyTitle()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestUser", IsBanned = false };

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            // Add mock services to test context.
            Context.Services.AddSingleton(mockDatabaseService.Object);
            Context.Services.AddSingleton(mockAuthService.Object);

            // Configure mock authorization.
            var authContext = Context.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Admin");
            #endregion Arrange


            #region Act
            // Render Index/Dashboard component.
            var cut = Context.RenderComponent<LogTitle>();

            var submitButton = cut.Find("button.mud-button-filled-primary");
            submitButton.Click();

            var titleError = cut.Find(".mud-input-error .me-auto").TextContent;
            #endregion Act


            #region Assert
            // Asserting the validation message is present. Message only shows on failed form submission.
            Assert.Equal("'Title' must not be empty.", titleError);
            #endregion Assert
        }

        /// <summary>
        /// Test case to test an authorized user attempting to submit a new watched item with a title that exceeds 128 characters.
        /// </summary>
        [Fact]
        public void TestToFail_AddNewWatchedItemExceedingCharacterLimit()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestUser", IsBanned = false };

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<WatchData, bool>>>()))
                .ReturnsAsync(new List<WatchData>());

            // Add mock services to test context.
            Context.Services.AddSingleton(mockDatabaseService.Object);
            Context.Services.AddSingleton(mockAuthService.Object);

            // Configure mock authorization.
            var authContext = Context.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Admin");
            #endregion Arrange


            #region Act
            // Render Index/Dashboard component.
            var cut = Context.RenderComponent<LogTitle>();

            // Get string with 129 characters to exceed 128 maximum length.
            StringBuilder sb = new();
            sb.Append('x', 129);

            var titleInput = cut.Find(".mud-autocomplete input");
            titleInput.Input(sb.ToString());
            titleInput.Click();

            var titleError = cut.Find(".mud-input-error .me-auto").TextContent;
            #endregion Act


            #region Assert
            // Asserting the validation message is present. Message only shows on failed form submission.
            Assert.True(titleError.Contains("The length of 'Title' must be 128 characters or fewer."));
            #endregion Assert
        }
    }
}
