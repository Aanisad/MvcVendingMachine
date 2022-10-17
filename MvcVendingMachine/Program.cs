using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MvcVendingMachine.Data;
using MvcVendingMachine.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MvcVendingMachineContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MvcVendingMachineContext") ?? throw new InvalidOperationException("Connection string 'MvcVendingMachineContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<MvcVendingMachineContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<MvcVendingMachineContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    option.AddPolicy("UserANDAdmin", policy => policy.RequireRole("User").RequireRole("Admin"));
    option.AddPolicy("Admin_CreateAccess", policy => policy.RequireRole("Admin").RequireClaim("Create", "True"));
    option.AddPolicy("Admin_Create_Edit_DeleteAccess", policy => policy.RequireRole("Admin")
    .RequireClaim("edit", "True")
    .RequireClaim("Create", "True")
    .RequireClaim("delete", "True"));

    option.AddPolicy("Admin_Create_Edit_DeleteAccess_OR_SuperAdmin", policy => policy.RequireAssertion(context => (
        context.User.IsInRole("Admin") && context.User.HasClaim(c => c.Type == "Create" && c.Value == "True")
        && context.User.HasClaim(c => c.Type == "Edit" && c.Value == "True")
        && context.User.HasClaim(c => c.Type == "Delete" && c.Value == "True")
        ) || context.User.IsInRole("SuperAdmin")
   ));
});

//Akses Ditolak     
//builder.Services.ConfigureApplicationCookie(opt =>
//{ 
//    opt.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Home/AccessDenied");
//});

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
});


app.Run();
