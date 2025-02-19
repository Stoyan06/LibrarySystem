using CloudinaryDotNet;
using LibrarySystem.Data;
using LibrarySystem.Data.Repository;
using LibrarySystem.Services;
using LibrarySystem.Services.IService;
using LibrarySystem.Utility;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddDbContext<ApplicationDbContext>
    //(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

builder.Services.AddScoped<CloudinaryService>();

var cloudinarySettings = builder.Configuration

                        .GetSection("Cloudinary")

                        .Get<CloudinarySettings>();



var account = new Account(cloudinarySettings.CloudName,

cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret);



var cloudinary = new Cloudinary(account);

builder.Services.AddSingleton(cloudinary);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
