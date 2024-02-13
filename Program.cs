using Ouroboros;

/*
// https://andrewlock.net/exploring-the-dotnet-8-preview-comparing-createbuilder-to-the-new-createslimbuilder-method/#what-s-missing-from-createslimbuilder-
var builder = WebApplication.CreateSlimBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/error");

app.UsePathBase("/ouroboros");

app.UseRouting();

app.MapRazorPages();

app.MapGet("/", () => Results.Redirect("/ouroboros/login"));

app.Run();*/

var res = await Headscale.InvokeHeadscale("nodes", "list", "-o", "json-line");
Console.WriteLine(res);