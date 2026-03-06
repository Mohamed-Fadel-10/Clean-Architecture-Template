using Api.Extensions;
using API.Extensions;
using API.SwaggerConfigurations;
using Application;
using Persistence;
using Persistence.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openap
builder.Services.ConfigureSwagger();
builder.Services.ConfigureIdentity();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.ConfigureJwtOptions(builder.Configuration);
builder.Services.ConfigureUserContext();

builder.Services.ConfigureCors(builder.Configuration);
var app = builder.Build();
// remove this comment when you need to seed users
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await IdentitySeeder.SeedUsersAsync(services);
//}
app.UseStaticFiles();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowOrigin");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

