using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Pages.User;
using System.Linq.Expressions;

namespace MediafulTest.Tests
{
    /// <summary>
    /// Test class with test cases for the Collection user page.
    /// </summary>
    public class CollectionTest : BaseTest
    {
        /// <summary>
        /// Test case to test an authorized user viewing their own collection.
        /// </summary>
        [Fact]
        public void TestToPass_ViewOwnCollection()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestUser", IsBanned = false };
            List<WatchData> testData = new()
            {
                new WatchData 
                { 
                    WatchDataId = 1,
                    Title = "Lost",
                    Status = 2,
                    TimesWatched = 0,
                    IsTvShow = true,
                    UserId = user.Id
                }
            };

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<WatchData, bool>>>()))
                .ReturnsAsync(testData);

            mockAuthService.Setup(x => x.GetUserId()).ReturnsAsync(user.Id);

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
            var cut = Context.RenderComponent<CollectionViewer>();

            var tableData = cut.FindAll("td")[0].TextContent;
            #endregion Act


            #region Assert
            // Checks if the WatchData is displayed.
            Assert.Equal(testData[0].Title, tableData);
            #endregion Assert
        }

        /// <summary>
        /// Test case to test viewing another user's collection.
        /// </summary>
        [Fact]
        public void TestToPass_ViewOtherUserCollection()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1337, UserName = "OtherTestUser", IsBanned = false };
            List<WatchData> testData = new()
            {
                new WatchData
                {
                    WatchDataId = 1,
                    Title = "Lost",
                    Status = 2,
                    TimesWatched = 0,
                    IsTvShow = true,
                    UserId = user.Id
                }
            };

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<WatchData, bool>>>()))
                .ReturnsAsync(testData);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            mockAuthService.Setup(x => x.GetUserId()).ReturnsAsync(user.Id);

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
            var cut = Context.RenderComponent<CollectionViewer>(parameters => parameters.Add(p => p.UserId, user.Id.ToString()));

            var tableData = cut.FindAll("td")[0].TextContent;

            var header = cut.Find("h1").TextContent;
            #endregion Act


            #region Assert
            // Checks if the WatchData is displayed.
            Assert.Equal(testData[0].Title, tableData);

            // Checks if the header contains the user's name. Header currently appears as "{username}'s Watch Collection".
            Assert.True(header.Contains("OtherTestUser"));
            #endregion Assert
        }

        /// <summary>
        /// Test case to test an authorized user deleted a logged title from their own collection.
        /// </summary>
        [Fact]
        public void TestToPass_DeleteLoggedDataFromCollection()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1337, UserName = "OtherTestUser", IsBanned = false };
            List<WatchData> testData = new()
            {
                new WatchData
                {
                    WatchDataId = 1,
                    Title = "Lost",
                    Status = 2,
                    TimesWatched = 0,
                    IsTvShow = true,
                    UserId = user.Id
                }
            };

            // Create mock services necessary for dependency injection.
            var mockDialogService = new Mock<IDialogService>();
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDialogService.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DialogOptions>()))
                .ReturnsAsync(true);

            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<WatchData, bool>>>()))
                .ReturnsAsync(testData);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            mockAuthService.Setup(x => x.GetUserId()).ReturnsAsync(user.Id);

            // Add mock services to test context.
            Context.Services.AddSingleton(mockDialogService.Object);
            Context.Services.AddSingleton(mockDatabaseService.Object);
            Context.Services.AddSingleton(mockAuthService.Object);

            // Configure mock authorization.
            var authContext = Context.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Admin");
            #endregion Arrange


            #region Act
            // Render Index/Dashboard component.
            var cut = Context.RenderComponent<CollectionViewer>();

            var tableData = cut.FindAll("td");

            bool hasInitialNotification = testData[0].Title == tableData[0].TextContent;

            // Delete logged item.
            var deleteButton = cut.Find($"#delete-{testData[0].WatchDataId}");
            deleteButton.Click();

            tableData = cut.FindAll("td");
            #endregion Act


            #region Assert
            // Ensures notification was initially displayed.
            Assert.True(hasInitialNotification);

            // Checks if notification is no longer displayed. 0 td elements will return if successfully deleted.
            Assert.True(tableData.Count() == 0);
            #endregion Assert
        }
    }
}
