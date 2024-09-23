using MyWebApp.Services;  // Import your services
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication.Cookies;  // For optional authentication
using Microsoft.AspNetCore.Builder;  // Add this if needed for using IApplicationBuilder

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register the DatabaseService with scoped lifetime
builder.Services.AddScoped<DatabaseService>();

// (Optional) Add authentication services if needed
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";  // Redirect to login page if unauthorized
        options.AccessDeniedPath = "/Account/AccessDenied";  // Redirect to Access Denied page if unauthorized
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");  // Global error handler for production
    app.UseHsts();  // Use HTTP Strict Transport Security for secure HTTPS communication
}

app.UseHttpsRedirection();  // Redirect HTTP requests to HTTPS
app.UseStaticFiles();  // Serve static files from wwwroot

// (Optional) Serve custom static files from a different directory, e.g., "StaticFiles"
// app.UseStaticFiles(new StaticFileOptions
// {
//     FileProvider = new PhysicalFileProvider(
//         Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles")),
//     RequestPath = "/StaticFiles"
// });

app.UseRouting();  // Enable routing middleware

// (Optional) Use authentication if required
app.UseAuthentication();  // Enable authentication
app.UseAuthorization();  // Enable authorization middleware

// (Optional) Log incoming requests and outgoing responses for debugging
app.Use(async (context, next) =>
{
    // Log request
    Console.WriteLine($"Incoming request: {context.Request.Method} {context.Request.Path}");
   
    await next.Invoke();  // Call the next middleware

    // Log response
    Console.WriteLine($"Outgoing response: {context.Response.StatusCode}");
});

// Map the default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();  // Start the application
