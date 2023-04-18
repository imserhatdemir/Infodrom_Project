using Infodrom.Server.Data;
using Infodrom.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

#region CORS setting for API

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "MyOrigins",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                              .AllowAnyHeader()
                                              .AllowAnyMethod();
                      });
});
#endregion




// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<IOrganizationService, OrganizationService>();
builder.Services.AddHttpClient();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseCors("MyOrigins");

app.MapDefaultControllerRoute();
app.MapBlazorHub();


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();