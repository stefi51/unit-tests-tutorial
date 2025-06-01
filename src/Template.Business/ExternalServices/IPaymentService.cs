namespace Template.Business.ExternalServices;

public interface IPaymentService
{
    public Task<bool> HasPendingPayments(string email);
}