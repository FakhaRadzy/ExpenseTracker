using ExpenseTracker.Infrastructure;
using System.Text.Json.Serialization;
using ExpenseTracker.Application;
using ExpenseTracker.Application.Common.Interfaces;
using Scalar.AspNetCore;
using ExpenseTracker.Api.Services;
using ExpenseTracker.Api.ErrorHandler;
using ExpenseTracker.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
    // Send and accept enums as text ("EWallet") instead of numbers (5)
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();              

builder.Services.AddOpenApiWithBearerAuth();

var app = builder.Build();

app.UseExceptionHandler(); // Must be FIRST so it catches errors from everything after

app.UseStatusCodePages();   // Empty 404/405 response become ProblemDetails too

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.AddPreferredSecuritySchemes("Bearer"));                // Interactive API docs at Scalar
}

app.UseHttpsRedirection();

app.UseAuthentication();                        // "Who are you?" (reads the token). Must come BEFORE

app.UseAuthorization();                         // "Are you allowed?" (check [Authorize])

app.MapControllers();

app.Run();
