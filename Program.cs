

// https://andrewlock.net/exploring-the-dotnet-8-preview-comparing-createbuilder-to-the-new-createslimbuilder-method/#what-s-missing-from-createslimbuilder-
var builder = WebApplication.CreateSlimBuilder(args);

// Add services to the container.
builder.Services.AddSession();
builder.Services.AddControllers();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/error");

app.UsePathBase("/ouroboros");

app.UseRouting();

app.UseSession();

app.MapControllers();
app.MapRazorPages();

app.MapGet("/", () => Results.Redirect("/ouroboros/dashboard"));

app.Run();