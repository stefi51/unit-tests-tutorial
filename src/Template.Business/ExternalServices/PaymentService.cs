namespace Template.Business.ExternalServices;

public class PaymentService: IPaymentService
{
    public Task<bool> HasPendingPayments(string email)
    {
        // HTTP call to external service
        throw new NotImplementedException();
    }
}