namespace Concertation.Banking.API.Features.Payments.Requests;

public record ProcessPaymentRequest(string CardNumber, decimal Amount);

