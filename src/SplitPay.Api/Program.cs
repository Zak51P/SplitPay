using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SplitPay.Api.Endpoints;
using SplitPay.Api.Requests;
using SplitPay.Application.Expenses;
using SplitPay.Application.Groups;
using SplitPay.Application.Members;
using SplitPay.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CreateGroupCommandHandler>();
builder.Services.AddScoped<AddMemberCommandHandler>();
builder.Services.AddScoped<CreateExpenseCommandHandler>();
builder.Services.AddScoped<GetGroupDetailsQueryHandler>();

builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Unexpected error",
            Detail = exceptionHandler?.Error.Message,
            Instance = context.Request.Path
        };
        context.Response.StatusCode = problem.Status!.Value;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(problem);
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGroup("/api")
    .MapSplitPayEndpoints();

app.Run();
