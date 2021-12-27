using ModuleCore.Module;
using PaymentLinkGeneration.Model;

namespace PaymentLinkOrderFilteration
{
    public class PaymentGatewayDetails : DBBase
    {
        public string PaymentGatewayName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public MailTemplate mailTemplate { get; set; }

    }
}
