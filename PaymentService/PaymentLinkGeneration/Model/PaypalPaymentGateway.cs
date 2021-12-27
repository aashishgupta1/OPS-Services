using PaymentLinkGeneration.Interface;

namespace PaymentLinkGeneration.Model
{
    class PaypalPaymentGateway : IPaymentGatewayBase
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public void connect()
        {

        }
    }
}
