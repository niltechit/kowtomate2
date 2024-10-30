using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using FastReport.Data;
using KowToMateAdmin;
using KowToMateAdmin.SignalRHub;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

builder.Services.AddControllersWithViews();
//authntication
// ******
// BLAZOR COOKIE Auth Code (begin)
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.Configure<FormOptions>(options =>
{
    // Set the limit to 128 MB
    options.MultipartBodyLengthLimit = 134217728;
});

//builder.Services.AddServerSideBlazor()
//                .AddHubOptions(options =>
//                {
//                    options.ClientTimeoutInterval = TimeSpan.FromHours(10);
//                    options.KeepAliveInterval = TimeSpan.FromSeconds(3);
//                    options.HandshakeTimeout = TimeSpan.FromMinutes(10);
//                });

builder.Services.AddServerSideBlazor()
.AddHubOptions(options =>
{
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(10);
    options.EnableDetailedErrors = true; // Set to true during development/debugging
    options.HandshakeTimeout = TimeSpan.FromMinutes(2);
    options.KeepAliveInterval = TimeSpan.FromMinutes(2);
    //options.MaximumParallelInvocationsPerClient = 20;
    options.MaximumReceiveMessageSize = 102400000000000;
    options.StreamBufferCapacity = 10000000; // Increase to 100 for larger files
    //options.ClientTimeoutInterval = TimeSpan.FromMinutes(10);
    //options.HandshakeTimeout = TimeSpan.FromMinutes(5);
    //options.KeepAliveInterval = TimeSpan.FromMinutes(5);
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.NoCompression;
});

//builder.Services.AddServerSideBlazor(options =>
//{
//	options.DetailedErrors = true;
//	//options.DisconnectedCircuitMaxRetained = 200;
//	//options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromHours(3);
//	options.JSInteropDefaultCallTimeout = TimeSpan.FromHours(1);
//	//options.MaxBufferedUnacknowledgedRenderBatches = 10;
//});
// BLAZOR COOKIE Auth Code (end)
// ******

// Fast Report Dependency Add
//FastReport.Utils.RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));

//builder.Services.AddSignalR(e =>
//{
//    e.MaximumReceiveMessageSize = 102400000000000;
//    e.ClientTimeoutInterval = TimeSpan.FromSeconds(10800);
//    e.KeepAliveInterval = TimeSpan.FromSeconds(10800);
//});

//For Notificaiton
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
//app.UseResponseCompression();
app.UseStaticFiles();
// Use Fast Report Dependency
//app.UseFastReport();
app.UseRouting();

app.MapDefaultControllerRoute();

//Authenticaion Start
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<BlazorCookieLoginMiddleware>();
//app.MapControllers();
//AUthencation end

//For Notificaiton
//app.UseResponseCompression();

//app.MapBlazorHub();
//app.MapFallbackToPage("/_Host");
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();            // new
    endpoints.MapBlazorHub();              // existing
    endpoints.MapFallbackToPage("/_Host"); // existing
    endpoints.MapHub<NotificationHub>("/notificationhub");
});

app.Run();


