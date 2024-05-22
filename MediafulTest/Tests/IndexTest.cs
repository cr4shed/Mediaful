using AngleSharp.Dom;
using Mediaful.Data.Models;
using Mediaful.Data.Services;
using System.Linq.Expressions;

namespace MediafulTest.Tests
{
    /// <summary>
    /// Test class with test cases for the Index/Dashboard user page.
    /// </summary>
    public class IndexTest : BaseTest
    {
        /// <summary>
        /// Test case to test notifications display for authorized users.
        /// </summary>
        [Fact]
        public void TestToPass_VerifyAuthorizedDashboardNotifications()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestSendingUser" };
            Notification notif = new Notification()
            {
                NotificationId = 1,
                SenderId = user.Id,
                Title = "Lost",
                Status = 3,
                TimesWatched = 1,
                Rating = 1,
                IsTvShow = true,
                TvSeason = null,
                TvEpisode = null,
                IsFavorite = true,
                Comment = "Test notification."
            };

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDatabaseService.Setup(x => x.GetNotifications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Notification>() { notif });

            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<NotificationReport, bool>>>()))
                .ReturnsAsync(new List<NotificationReport>());

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
            var cut = Context.RenderComponent<Mediaful.Pages.User.Index>();
            // Check for a notification element in the DOM with ID "notification-1".
            // That is the test notification created in the Arrange section.
            var dashboardNotification = cut.Find($"#notification-{notif.NotificationId}");

            #endregion Act


            #region Assert
            // If the notification exists, the dashboard is functioning as expected.
            Assert.NotNull(dashboardNotification);
            #endregion Assert
        }

        /// <summary>
        /// Test case to test authorized users reporting a notification.
        /// </summary>
        [Fact]
        public void TestToPass_ReportNotification()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestSendingUser" };
            Notification notif = new Notification()
            {
                NotificationId = 1,
                SenderId = user.Id,
                Title = "Lost",
                Status = 3,
                TimesWatched = 1,
                Rating = 1,
                IsTvShow = true,
                TvSeason = null,
                TvEpisode = null,
                IsFavorite = true,
                Comment = "Test notification."
            };

            // Create mock services necessary for dependency injection.
            var mockDialogService = new Mock<IDialogService>();
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            mockDialogService.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DialogOptions>()))
                            .ReturnsAsync(true);

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDatabaseService.Setup(x => x.GetNotifications(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new List<Notification>() { notif });

            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<NotificationReport, bool>>>()))
                .ReturnsAsync(new List<NotificationReport>());

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

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
            var cut = Context.RenderComponent<Mediaful.Pages.User.Index>();

            // Click report button.
            var reportButton = cut.Find($"#report-{notif.NotificationId}");
            reportButton.Click();
            #endregion Act


            #region Assert
            // After a user reports a notification the report button is disabled
            // to prevent duplicates. Check for disabled report button to pass.
            Assert.True(reportButton.IsDisabled());
            #endregion Assert
        }
    }
}