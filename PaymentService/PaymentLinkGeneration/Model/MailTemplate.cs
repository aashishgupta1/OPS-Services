using ModuleCore.Module;

namespace PaymentLinkGeneration.Model
{
    public class MailTemplate : DBBase
    {
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
        public string MailSignature { get; set; }
    }
}
