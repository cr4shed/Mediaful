using Mediaful.Areas.Identity;
using Mediaful.Data;
using Mediaful.Data.Models;
using Mediaful.Data.Services;
using Mediaful.Shared;
using Mediaful.Shared.Layout;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MudBlazor;
using MudBlazor.Services;

namespace Mediaful
{
    /// <summary>
    /// Default Program class.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">Args.</param>
        /// <exception cref="InvalidOperationException">Exception.</exception>
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<MediafulDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>
                (options => {
                    options.SignIn.RequireConfirmedAccount = true;
                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<MediafulDbContext>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            builder.Services.AddTransient<IEmailSender, EmailSender>(); // COMMENT THIS TO DISABLE IDENTITY EMAILS.
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDatabaseService, DatabaseService>();

            #region MudBlazor
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopRight;
            });
            #endregion

            #region Custom Services
            builder.Services.AddScoped<LayoutState>();
            builder.Services.AddScoped<DatabaseService>();
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<DbContextFactory>();
            DbContextFactory.SetConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"));
            #endregion

            #region Localization Service
            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

            var supportedCultures = new[] { "en-US" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            #endregion

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();

            }

            // Initialize database.
            using (var scope = app.Services.CreateScope())
            {
                DatabaseInitializer.SeedDatabase(scope.ServiceProvider).Wait();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }
    }
}