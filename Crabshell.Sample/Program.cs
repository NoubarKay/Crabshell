using Crabshell.Admin;
using Crabshell.Core;
using Crabshell.Data;
using Crabshell.Sample.Components;
using Radzen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRadzenComponents();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCrabshellCore(typeof(Program).Assembly);
builder.Services.AddCrabshellData<CrabshellDb>(builder.Configuration.GetConnectionString("DefaultConnection")!);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(AdminExtensions.Assembly);

app.MapCrabshellAdmin();

await app.UseCrabshellDataAsync();

app.Run();


//TODO: Be able too add DB Context NAME
//TODO: Fluent api generate the constraints in db context, or make a partial
//TODO: In email validate it in more detail
//TODO: Enum value title override in select field [JsonStringEnumMemberName("House")]
//TODO: Enum Flag multiple values / Aliases of other values
//TODO: Many too many table in multiple dropdown
//TODO: Pagination using cursor (Optional)
//TODO: Add page query param to grid page
//TODO: Static analyzer
//TODO: REGEX is not working on text input
//TODO: Research form builder
//TODO: Split button, with saving abilities.
//TODO: RECURRING like hangfire
//TODO: ADD COLLAPSIBLE TO FIELD GROUP and default collapse state (Also check tabs)
//TODO: Dynamic enums