namespace DietApp.Core.Requests;

public record ByIdRequest(Guid Id) : IRequest;
