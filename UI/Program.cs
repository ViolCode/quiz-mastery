using BLL.Configuration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddBusinessLayer(builder.Configuration);

builder.Services.AddIdentityCore<DAL.Models.ApplicationUser>()
    .AddRoles<Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<DAL.ApplicationDbContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/", context =>
{
    context.Response.Redirect("/Identity/Account/Login");
    return Task.CompletedTask;
});

app.Run();
