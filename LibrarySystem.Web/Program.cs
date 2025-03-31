namespace LibrarySystem.Web
{
    using CloudinaryDotNet;
    using LibrarySystem.Data;
    using LibrarySystem.Data.Repository;
    using LibrarySystem.Models;
    using LibrarySystem.Services;
    using LibrarySystem.Services.IService;
    using LibrarySystem.Utility;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Threading.Tasks;

    class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Configure Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            // Register services
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
            builder.Services.AddScoped<ISectionService, SectionService>();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<ISearchService, SearchService>();

            builder.Services.AddScoped<CloudinaryService>();

            // Configure Cloudinary
            var cloudinarySettings = builder.Configuration.GetSection("Cloudinary").Get<CloudinarySettings>();
            var account = new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);
            var cloudinary = new Cloudinary(account);
            builder.Services.AddSingleton(cloudinary);

            // Configure Identity
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });

            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure middleware
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            // Create roles and admin user on startup
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                await CreateRoles(services);
                await CreateAdmin(services);
            }

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync(); // Ensure `RunAsync` is used instead of `Run()`
        }

        private static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { SD.AdminRole, SD.LibrarianRole, SD.UserRole };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        /*
        private static async Task CreateAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var adminEmail = "stoyanzlankov06@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var identityUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                var result = await userManager.CreateAsync(identityUser, "AdminPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(identityUser, SD.AdminRole);
                    var user = new User
                    { FirstName = "Стоян", MiddleName = "Пенков", LastName = "Зланков", IdentityUserId = identityUser.Id };
                }
            }
        }
        */

        private static async Task CreateAdmin(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userService = serviceProvider.GetRequiredService<IService<User>>();

            // Ensure the Admin role exist

            // Get all users in the Admin role
            var admins = await userManager.GetUsersInRoleAsync(SD.AdminRole);

            // If there's already at least one admin, do nothing
            if (admins.Any())
                return;

            // Otherwise, create the default admin
            var adminEmail = "stoyanzlankov06@gmail.com";
            var identityUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            var result = await userManager.CreateAsync(identityUser, "AdminPassword123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(identityUser, SD.AdminRole);

                var user = new User
                {
                    FirstName = "Стоян",
                    MiddleName = "Пенков",
                    LastName = "Зланков",
                    IdentityUserId = identityUser.Id
                };

                await userService.AddAsync(user);
            }
        }
    }
}