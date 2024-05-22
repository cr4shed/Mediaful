using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Pages.Admin;
using System.Linq.Expressions;

namespace MediafulTest.Tests
{
    /// <summary>
    /// Test class with test cases for the Notification Reports admin page.
    /// </summary>
    public class NotificationReportsTest : BaseTest
    {
        /// <summary>
        /// Test case to test marking a comment as unsafe, removing it, and disabling the sending user..
        /// </summary>
        [Fact]
        public void TestToPass_DeleteCommentAndDisableUser()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestUser", IsBanned = false };
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
            List<NotificationReport> testData = new()
            {
                new NotificationReport
                {
                    ReportId = 1,
                    NotificationId = notif.NotificationId,
                    ReporterId = user.Id,
                    Status = 0,
                    Notification = notif
                }
            };

            // Create mock services necessary for dependency injection.
            var mockDialogService = new Mock<IDialogService>();
            var mockDatabaseService = new Mock<IDatabaseService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDialogService.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DialogOptions>()))
                .ReturnsAsync(true);

            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<NotificationReport, bool>>>(), It.IsAny<Expression<Func<NotificationReport, object>>>()))
                .ReturnsAsync(testData);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<Notification, bool>>>()))
                .ReturnsAsync(notif);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            // Add mock services to test context.
            Context.Services.AddSingleton(mockDialogService.Object);
            Context.Services.AddSingleton(mockDatabaseService.Object);

            // Configure mock authorization.
            var authContext = Context.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Admin");
            #endregion Arrange


            #region Act
            // Render Index/Dashboard component.
            var cut = Context.RenderComponent<NotificationReports>();

            var disableButton = cut.Find($"#disable-{testData[0].ReportId}");
            disableButton.Click();

            var tableData = cut.FindAll("td");
            #endregion Act


            #region Assert
            // Checks if report is deleted. 0 td elements will return if successfully actioned.
            Assert.True(tableData.Count() == 0);
            #endregion Assert
        }

        /// <summary>
        /// Test case to test marking a comment as unsafe and removing it.
        /// </summary>
        [Fact]
        public void TestToPass_DeleteComment()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestUser", IsBanned = false };
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
            List<NotificationReport> testData = new()
            {
                new NotificationReport
                {
                    ReportId = 1,
                    NotificationId = notif.NotificationId,
                    ReporterId = user.Id,
                    Status = 0,
                    Notification = notif
                }
            };

            // Create mock services necessary for dependency injection.
            var mockDialogService = new Mock<IDialogService>();
            var mockDatabaseService = new Mock<IDatabaseService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDialogService.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DialogOptions>()))
                .ReturnsAsync(true);

            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<NotificationReport, bool>>>(), It.IsAny<Expression<Func<NotificationReport, object>>>()))
                .ReturnsAsync(testData);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<NotificationReport, bool>>>()))
                .ReturnsAsync(testData[0]);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<Notification, bool>>>()))
                .ReturnsAsync(notif);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            // Add mock services to test context.
            Context.Services.AddSingleton(mockDialogService.Object);
            Context.Services.AddSingleton(mockDatabaseService.Object);

            // Configure mock authorization.
            var authContext = Context.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Admin");
            #endregion Arrange


            #region Act
            // Render Index/Dashboard component.
            var cut = Context.RenderComponent<NotificationReports>();

            var unsafeButton = cut.Find($"#unsafe-{testData[0].ReportId}");
            unsafeButton.Click();

            // Status text.
            var status = cut.FindAll("td")[2].TextContent;

            // Comment text.
            var comment = cut.FindAll("td")[4].TextContent;
            #endregion Act


            #region Assert
            // Checks if report has been successfully actioned.
            // Status changes from Pending (0) to Unsafe (3).
            Assert.Equal("Unsafe", status);

            // Checks if comment has been deleted.
            Assert.Equal("[ COMMENT DELETED ]", comment);
            #endregion Assert
        }

        /// <summary>
        /// Test case to test marking a comment as safe and taking no further action on the report.
        /// </summary>
        [Fact]
        public void TestToPass_MarkSafe()
        {
            #region Arrange
            // Create test data.
            ApplicationUser user = new ApplicationUser() { Id = 1, UserName = "TestUser", IsBanned = false };
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
            List<NotificationReport> testData = new()
            {
                new NotificationReport
                {
                    ReportId = 1,
                    NotificationId = notif.NotificationId,
                    ReporterId = user.Id,
                    Status = 0,
                    Notification = notif
                }
            };

            // Create mock services necessary for dependency injection.
            var mockDialogService = new Mock<IDialogService>();
            var mockDatabaseService = new Mock<IDatabaseService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDialogService.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DialogOptions>()))
                .ReturnsAsync(true);

            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<NotificationReport, bool>>>(), It.IsAny<Expression<Func<NotificationReport, object>>>()))
                .ReturnsAsync(testData);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<NotificationReport, bool>>>()))
                .ReturnsAsync(testData[0]);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<Notification, bool>>>()))
                .ReturnsAsync(notif);

            mockDatabaseService.Setup(x => x.GetRecord(It.IsAny<Expression<Func<ApplicationUser, bool>>>()))
                .ReturnsAsync(user);

            // Add mock services to test context.
            Context.Services.AddSingleton(mockDialogService.Object);
            Context.Services.AddSingleton(mockDatabaseService.Object);

            // Configure mock authorization.
            var authContext = Context.AddTestAuthorization();
            authContext.SetAuthorized("TestUser");
            authContext.SetRoles("Admin");
            #endregion Arrange


            #region Act
            // Render Index/Dashboard component.
            var cut = Context.RenderComponent<NotificationReports>();

            var unsafeButton = cut.Find($"#safe-{testData[0].ReportId}");
            unsafeButton.Click();

            // Status text.
            var status = cut.FindAll("td")[2].TextContent;
            #endregion Act


            #region Assert
            // Checks if report has been successfully actioned.
            // Status changes from Pending (0) to Unsafe (3).
            Assert.Equal("Safe", status);
            #endregion Assert
        }
    }
}
