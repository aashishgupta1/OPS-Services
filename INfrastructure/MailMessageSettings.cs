using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace OPSService.Infrastructure
{
    [Serializable]
    public class MailMessageSettings
    {
        #region Message Settings
        public bool IsBodyHtml { get; set; }

        public string FromEmailAddress { get; set; }

        public IList<string> ToEmailAddress { get; set; }

        public IList<string> CcEmailAddress { get; set; }

        public IList<string> BccEmailAddress { get; set; }

        public string EmailSubject { get; set; }

        public string EmailMessage { get; set; }

        public List<Attachment> SystemAttachments { get; set; }
        public string Key { get; set; }
        #endregion
    }
}
