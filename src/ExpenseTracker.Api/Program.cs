using ExpenseTracker.Infrastructure;
using System.Text.Json.Serialization;
using ExpenseTracker.Application;
using ExpenseTracker.Application.Common.Interfaces;
using Scalar.AspNetCore;
using ExpenseTracker.Api.Services;
using ExpenseTracker.Api.ErrorHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
    // Send and accept enums as text ("EWallet") instead of numbers (5)
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ICurrentUserService, DevCurrentUserService>();               // TEMPORARY until Phase 5

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler(); // Must be FIRST so it catches errors from everything after

app.UseStatusCodePages();   // Empty 404/405 response become ProblemDetails too

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();                // Interactive API docs at Scalar
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
