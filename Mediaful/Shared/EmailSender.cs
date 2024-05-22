using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace Mediaful.Shared
{
    /// <summary>
    /// Class which handles emails for the scaffolded Identity pages.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        /// <summary>
        /// Configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor. Uses dependency injection from Program.cs.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public EmailSender(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Method to send an email.
        /// </summary>
        /// <param name="email">Destination email.</param>
        /// <param name="subject">Subject line.</param>
        /// <param name="htmlMessage">Message.</param>
        /// <returns>Task.</returns>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Create SMTP client with credentials from secrets.
            SmtpClient client = new SmtpClient
            {
                Port = 587,
                Host = Configuration["Mail:Provider"],
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Configuration["Mail:UserName"], Configuration["Mail:Password"])
                // Credentials need to be manually entered or stored in secrets for deployment.
            };

            // Send email.
            return client.SendMailAsync(
                new MailMessage(Configuration["Mail:NoReplyAddress"], email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
}
