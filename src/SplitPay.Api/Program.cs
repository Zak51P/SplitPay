using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
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

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapPost("/groups", async (CreateGroupCommand command, CreateGroupCommandHandler handler, CancellationToken ct) =>
{
    var id = await handler.Handle(command, ct);
    return Results.Created($"/groups/{id}", new { id });
});

app.MapPost("/groups/{groupId:guid}/members", async (Guid groupId, AddMemberCommand command, AddMemberCommandHandler handler, CancellationToken ct) =>
{
    var request = command with { GroupId = groupId };
    var id = await handler.Handle(request, ct);
    return Results.Created($"/groups/{groupId}/members/{id}", new { id });
});

app.MapPost("/groups/{groupId:guid}/expenses", async (Guid groupId, CreateExpenseCommand command, CreateExpenseCommandHandler handler, CancellationToken ct) =>
{
    var request = command with { GroupId = groupId };
    var id = await handler.Handle(request, ct);
    return Results.Created($"/groups/{groupId}/expenses/{id}", new { id });
});

app.MapGet("/groups/{groupId:guid}", async (Guid groupId, GetGroupDetailsQueryHandler handler, CancellationToken ct) =>
{
    var result = await handler.Handle(new GetGroupDetailsQuery(groupId), ct);
    return result is null ? Results.NotFound() : Results.Ok(result);
});

app.Run();
