
// https://andrewlock.net/exploring-the-dotnet-8-preview-comparing-createbuilder-to-the-new-createslimbuilder-method/#what-s-missing-from-createslimbuilder-
var builder = WebApplication.CreateSlimBuilder(args);

// Add services to the container.
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/oopsie");

app.UsePathBase("/ouroboros");

app.UseRouting();

app.UseSession();

app.MapDefaultControllerRoute();

app.MapGet("/",       () => Results.Redirect("/ouroboros/dashboard"));
app.MapGet("/oopsie", () => "sorry, something broke");

app.Run();