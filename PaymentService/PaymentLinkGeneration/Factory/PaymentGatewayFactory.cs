using ModuleCore;
using PaymentLinkGeneration.Interface;
using PaymentLinkGeneration.Model;

namespace PaymentLinkGeneration.Factory
{
    public static class PaymentGatewayFactory
    {
        public static IPaymentGatewayBase GetPaymentGateway(PaymentGateway PaymentGateway)
        {
            IPaymentGatewayBase result = null;
            switch (PaymentGateway)
            {
                case PaymentGateway.PAYPAL:
                    result = new PaypalPaymentGateway();
                    break;
                default:
                    result = new PaypalPaymentGateway();
                    break;
            }
            return result;
        }
    }
}
