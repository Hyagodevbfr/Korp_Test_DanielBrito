namespace Korp.Billing.Infrastructure.Http;

internal record ProblemDetailsResponse(string? Title, string? Detail, int? Status);
