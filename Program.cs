using StoreMongoApp.Models;
using StoreMongoApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.Configure<MongoDBSettings>(
    builder.Configuration.GetSection("MongoDB")
);

builder.Services.AddSingleton<ProductService>();

builder.Services.Configure<GeminiSettings>(
    builder.Configuration.GetSection("Gemini")
);

builder.Services.AddHttpClient<GeminiProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
