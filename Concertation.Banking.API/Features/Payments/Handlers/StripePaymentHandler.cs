using Concertation.Banking.API.Features.Payments.Requests;

namespace Concertation.Banking.API.Features.Payments.Handlers;

public class StripePaymentHandler
{
    //public async Task Handle(ProcessPaymentRequest request)
    //{
    //    var chargeOptions = new ChargeCreateOptions
    //    {
    //        Amount = (long)(request.Amount * 100),
    //        Currency = "EUR",
    //        Source = request.CardNumber
    //    };

    //    var service = new ChargeService();
    //    var charge = await service.CreateAsync(chargeOptions);

    //    if (charge == null || !charge.Paid)
    //        throw new Exception("Paiement refusé");
    //}
}
