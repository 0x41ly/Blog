using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Blog.Areas.Identity.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blog.Data.Repository;
using Blog.Data.FileManager;
using Blog.Data;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("BlogDbContextConnection");



builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<BlogUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BlogDbContext>();

builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IFileManager, FileManager>();
// Add services to the container.
builder.Services.AddControllersWithViews();

// Add AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.Configure<IdentityOptions>(options =>
{
    // Default User settings.
    options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
    options.User.RequireUniqueEmail = false;

});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.Cookie.Name = "Cookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.LoginPath = "/Identity/Account/Login";
    // ReturnUrlParameter requires 
    //using Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});

builder.Services.Configure<PasswordHasherOptions>(option =>
{
    option.IterationCount = 12000;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

var scope = app.Services.CreateScope();
try
{
    var ctx = scope.ServiceProvider.GetRequiredService<BlogDbContext>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<BlogUser>>();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    ctx.Database.EnsureCreated();

    var adminRole = new IdentityRole("Admin");
    if (!ctx.Roles.Any())
    {
        //create a role
        roleMgr.CreateAsync(adminRole).GetAwaiter().GetResult();
    }
    if (!ctx.Users.Any(u => u.UserName == "admin"))
    {
        //create an admin
        var adminUser = new BlogUser
        {
            UserName = "admin",
            Email = "admin@blog.com",
            FirstName = "SIKI",
            LastName = "MIKI",
            Gender = "Male",
            PlanType = "Basic",
            DOB = new DateTime(2000, 1, 1)
        };
        var result = userMgr.CreateAsync(adminUser, "6#B9[*g,f=x[V+7t").GetAwaiter().GetResult();
        var code = await userMgr.GenerateEmailConfirmationTokenAsync(adminUser);
        var ConfirmationResult = await userMgr.ConfirmEmailAsync(adminUser, code);
        if (ConfirmationResult.Succeeded)
        {
            Console.WriteLine("Successfully Done");
        }
        else
        {
            Console.WriteLine("Something Went Wrong");
        }
        //add role to user
        userMgr.AddToRoleAsync(adminUser, adminRole.Name).GetAwaiter().GetResult();
    }

}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
