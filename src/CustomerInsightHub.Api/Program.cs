using CustomerInsightHub.Application;
using CustomerInsightHub.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // In development the React dev server proxies plain http requests to this API.
    // Redirecting them to https would send the browser to a different origin and CORS would block it.
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
