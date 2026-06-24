using EventHub.Api.Http;
using EventHub.Api.Mapping;
using EventHub.Application.Events.Commands;
using EventHub.Contracts.Events;
using MediatR;

namespace EventHub.Api.Endpoints;

internal sealed class TicketTypesEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/events/{eventId}/ticket-types", AddTicketType)
            .WithName("AddTicketType")
            .WithTags("TicketTypes")
            .RequireAuthorization()
            .RequireCompleteJsonBody<AddTicketTypeRequest>()
            .Accepts<AddTicketTypeRequest>("application/json")
            .Produces<AddTicketTypeResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> AddTicketType(
        int eventId,
        AddTicketTypeRequest request,
        ISender sender)
    {
        var command = new AddTicketTypeCommand(
            eventId,
            request.Name,
            request.PriceAmount,
            request.PriceCurrency,
            request.Capacity);

        var result = await sender.Send(command);

        if (!result.IsSuccess)
        {
            return result.ToHttpResult();
        }

        var ticketType = result.Value!;

        return Results.Json(
            new AddTicketTypeResponse(
                ticketType.TicketTypeId,
                ticketType.Name,
                ticketType.PriceAmount,
                ticketType.PriceCurrency,
                ticketType.Capacity,
                ticketType.Sold,
                ticketType.Reserved,
                ticketType.CreatedAt),
            statusCode: StatusCodes.Status201Created);
    }
}
