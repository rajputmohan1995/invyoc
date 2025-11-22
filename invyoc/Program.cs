using DinkToPdf;
using DinkToPdf.Contracts;
using invyoc.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddScoped<PdfService>();

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);



builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // compress even on HTTPS
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();

    // Optional: only compress certain MIME types
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes
            .Where(m => m != "application/pdf") // 🔥 exclude PDFs
            .Concat([
                "application/json",
                "text/plain",
                "text/css",
                "application/javascript",
                "text/html",
                "image/svg+xml"
            ]);
});

// Optional: Customize compression level
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxResponseBufferSize = null; // unlimited
    options.Limits.MaxRequestBodySize = 104857600; // 100 MB request body
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});


builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http1;
        listenOptions.UseHttps();
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    if (!string.IsNullOrWhiteSpace(path))
    {
        if (path.StartsWith("/invoiceJson") || path.Equals("/invoice-template.html", StringComparison.OrdinalIgnoreCase))
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync("Not Found!");
        }
    }

    await next();
});

app.UseMiddleware<CustomErrorHandlingMiddleware>();

app.UseResponseCompression();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseHttpsRedirection();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();