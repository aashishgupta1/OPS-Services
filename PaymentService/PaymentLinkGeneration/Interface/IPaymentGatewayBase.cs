namespace PaymentLinkGeneration.Interface
{
    public interface IPaymentGatewayBase
    {
        string UserName { get; set; }
        string Password { get; set; }
        void connect();
    }
}
