using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using StampSystem.Data;
using StampSystem.Models;
using StampSystem.Utility;
using System.Drawing.Text;

    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    // Models and DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(Options => {
        Options.LoginPath = "/Identity/Account/Login";
        Options.LogoutPath = "/Identity/Account/Logout";
        Options.AccessDeniedPath = "/Identity/Account/AccessDenied";
        
    });

builder.Services.AddRazorPages();
    builder.Services.AddScoped<IEmailSender, EmailSender>();

// Fix: Correctly configure Identity options

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
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapRazorPages();
    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

await SeedHRUserAsync(app);

app.Run();



async Task SeedHRUserAsync(IApplicationBuilder app)
{
    using var scope = app.ApplicationServices.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    if (!await roleManager.RoleExistsAsync("HR"))
    {
        await roleManager.CreateAsync(new IdentityRole("HR"));
    }

    var hrUser = await userManager.FindByEmailAsync("HR@gmail.com");
    if (hrUser == null)
    {
        hrUser = new ApplicationUser
        {
            UserName = "HR@gmail.com",
            Email = "HR@gmail.com",
            FullName = "HR Manager",
            Status = "Approved",
            Role = "HR",
            PhoneNumber = "0000000000",
            NationalID = "0000000000",
            EmployeeId = 76161,
            AdministrationId = 1, // قيم موجودة فعلياً في قاعدة البيانات
            SectionId = 1,        // قيم موجودة فعلياً
            UnitId = 1             // قيم موجودة فعلياً
        };

        var result = await userManager.CreateAsync(hrUser, "HrDefault123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(hrUser, "HR");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(hrUser, "HR"))
            await userManager.AddToRoleAsync(hrUser, "HR");

        if (hrUser.Status != "Approved")
        {
            hrUser.Status = "Approved";
            await userManager.UpdateAsync(hrUser);
        }
    }
}
// Fix for CS0106: Remove 'private' modifier from top-level method
// Fix for CS1998: Add 'await' operator to use asynchronous RoleManager API
