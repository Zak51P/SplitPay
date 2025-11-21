using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using SplitPay.Api.Requests;
using SplitPay.Application.Expenses;
using SplitPay.Application.Groups;
using SplitPay.Application.Members;

namespace SplitPay.Api.Endpoints;

public static class SplitPayEndpoints
{
    public static RouteGroupBuilder MapSplitPayEndpoints(this RouteGroupBuilder group)
    {
        group.MapGet("/health", () => Results.Ok(new { status = "ok" }))
            .WithName("Health")
            .WithOpenApi();

        group.MapPost("/groups", async Task<Results<Created<object>, ValidationProblem>> (
            CreateGroupRequest request,
            CreateGroupCommandHandler handler,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { "name", new[] { "Name is required" } } });

            var id = await handler.Handle(new CreateGroupCommand(request.Name.Trim()), ct);
            return TypedResults.Created($"/api/groups/{id}", new { id });
        })
        .WithName("CreateGroup")
        .WithOpenApi();

        group.MapPost("/groups/{groupId:guid}/members", async Task<Results<Created<object>, ValidationProblem>> (
            Guid groupId,
            AddMemberRequest request,
            AddMemberCommandHandler handler,
            CancellationToken ct) =>
        {
            if (string.IsNullOrWhiteSpace(request.DisplayName))
                return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { "displayName", new[] { "DisplayName is required" } } });

            var command = new AddMemberCommand(groupId, request.DisplayName.Trim());
            var id = await handler.Handle(command, ct);
            return TypedResults.Created($"/api/groups/{groupId}/members/{id}", new { id });
        })
        .WithName("AddMember")
        .WithOpenApi();

        group.MapPost("/groups/{groupId:guid}/expenses", async Task<Results<Created<object>, ValidationProblem, BadRequest<string>>> (
            Guid groupId,
            CreateExpenseRequest request,
            CreateExpenseCommandHandler handler,
            CancellationToken ct) =>
        {
            try
            {
                var command = request.ToCommand(groupId);
                var id = await handler.Handle(command, ct);
                return TypedResults.Created($"/api/groups/{groupId}/expenses/{id}", new { id });
            }
            catch (ValidationException ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { "validation", new[] { ex.Message } } });
            }
            catch (ArgumentException ex)
            {
                return TypedResults.ValidationProblem(new Dictionary<string, string[]> { { ex.ParamName ?? "request", new[] { ex.Message } } });
            }
            catch (InvalidOperationException ex)
            {
                return TypedResults.BadRequest(ex.Message);
            }
        })
        .WithName("CreateExpense")
        .WithOpenApi();

        group.MapGet("/groups/{groupId:guid}", async Task<Results<Ok<object>, NotFound>> (
            Guid groupId,
            GetGroupDetailsQueryHandler handler,
            CancellationToken ct) =>
        {
            var dto = await handler.Handle(new GetGroupDetailsQuery(groupId), ct);
            return dto is null ? TypedResults.NotFound() : TypedResults.Ok(dto);
        })
        .WithName("GetGroup")
        .WithOpenApi();

        return group;
    }
}
