using AngleSharp.Dom;
using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Pages.Admin;
using System.Linq.Expressions;
using System.Text;

namespace MediafulTest.Tests
{
    /// <summary>
    /// Test class with tests for the Announcements page.
    /// </summary>
    public class AnnouncementsTest : BaseTest
    {
        /// <summary>
        /// Test case to test sending an announcement.
        /// </summary>
        [Fact]
        public void TestToPass_SendAnnouncement()
        {
            #region Arrange
            // Create test data.
            string announcementMsg = "Test Announcement";

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

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
            var cut = Context.RenderComponent<Announcements>();

            var initialTdCount = cut.FindAll("td").Count();

            var textInput = cut.Find("textarea");
            textInput.Change(announcementMsg);

            var submitButton = cut.Find("#submit");
            submitButton.Click();

            var afterTdCount = cut.FindAll("td").Count();
            var tdAnnouncementMsg = cut.FindAll("td")[1].TextContent;
            #endregion Act


            #region Assert
            // Check that td elements have increased. This signifies that a new row has been added.
            Assert.True(afterTdCount > initialTdCount);

            // Verify announcement message is present.
            Assert.Equal(announcementMsg, tdAnnouncementMsg);
            #endregion Assert
        }

        /// <summary>
        /// Test case to test sending an announcement.
        /// </summary>
        [Fact]
        public void TestToPass_DeleteAnnouncement()
        {
            #region Arrange
            // Create test data.
            Notification announcement = new Notification()
            {
                NotificationId = 1,
                SenderId = 0,
                Title = "[SYSTEM ANNOUNCEMENT]",
                TimesWatched = 0,
                Rating = 0,
                IsTvShow = false,
                IsFavorite = false,
                Comment = "Test Announcement"
            };

            // Create mock services necessary for dependency injection.
            var mockDialogService = new Mock<IDialogService>();
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

            // Simulate DatabaseService calls and returns to mimic real database interactions.
            mockDialogService.Setup(x => x.ShowMessageBox(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DialogOptions>()))
                .ReturnsAsync(true);

            mockDatabaseService.Setup(x => x.GetRecords(It.IsAny<Expression<Func<Notification, bool>>>()))
                .ReturnsAsync(new List<Notification>()
                {
                    announcement
                });

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
            var cut = Context.RenderComponent<Announcements>();

            var initialTdCount = cut.FindAll("td").Count();

            var deleteButton = cut.Find($"#delete-{announcement.NotificationId}");
            deleteButton.Click();

            var afterTdCount = cut.FindAll("td").Count();
            #endregion Act


            #region Assert
            // Verify initial td elements are above 0 which means there is a row.
            Assert.True(initialTdCount > 0);

            // Verify that after deletion there are no td elements which means there are no rows.
            Assert.True(afterTdCount == 0);
            #endregion Assert
        }

        /// <summary>
        /// Test case to test sending an empty announcement.
        /// </summary>
        [Fact]
        public void TestToFail_SendEmptyAnnouncement()
        {
            #region Arrange
            // Create test data.
            string announcementMsg = "";

            // Create mock services necessary for dependency injection.
            var mockDatabaseService = new Mock<IDatabaseService>();
            var mockAuthService = new Mock<IAuthService>();

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
            var cut = Context.RenderComponent<Announcements>();

            var textInput = cut.Find("textarea");
            textInput.SetInnerText(announcementMsg);
            textInput.Input(announcementMsg);

            // Attempt to submit.
            var submitButton = cut.Find("#submit");
            submitButton.Click();

            var titleError = cut.Find(".mud-input-error .me-auto").TextContent;
            #endregion Act


            #region Assert
            // Verify error message is present.
            Assert.Equal("'Announcement' must not be empty.", titleError);
            #endregion Assert
        }
    }
}
