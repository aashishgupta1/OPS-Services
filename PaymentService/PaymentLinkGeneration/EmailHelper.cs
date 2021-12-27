using OPSService.Infrastructure;
using OPSService.Infrastructure.Loggging;
using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace PaymentLinkGeneration
{
    public class EmailHelper
    {
        /// <summary>
        /// Sends email message.
        /// </summary>
        /// <param name="emailModel">emailModel</param>
        public static void SendEmailMessage(MailServerSettings emailModel, MailMessageSettings messageModel)
        {
            try
            {
                messageModel.FromEmailAddress = emailModel.SmtpEmail;
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(messageModel.FromEmailAddress);
                mailMessage.Subject = messageModel.EmailSubject;
                mailMessage.Body = messageModel.EmailMessage;

                // adding to/cc/bcc recepients
                if (messageModel.ToEmailAddress.Count > 0)
                {
                    foreach (string address in messageModel.ToEmailAddress)
                    {
                        mailMessage.To.Add(new MailAddress(address));
                    }
                }

                if (messageModel.CcEmailAddress != null)
                {
                    foreach (string address in messageModel.CcEmailAddress)
                    {
                        mailMessage.CC.Add(new MailAddress(address));
                    }
                }

                if (messageModel.BccEmailAddress != null)
                {
                    foreach (string address in messageModel.BccEmailAddress)
                    {
                        mailMessage.Bcc.Add(new MailAddress(address));
                    }
                }

                mailMessage.BodyEncoding = UnicodeEncoding.Default;
                mailMessage.IsBodyHtml = messageModel.IsBodyHtml;

                if (messageModel.SystemAttachments != null)
                {
                    foreach (var item in messageModel.SystemAttachments)
                    {
                        mailMessage.Attachments.Add(item);
                    }
                }

                // Create and init the SMTP Client
                int port = int.Parse(emailModel.SmtpPort);
                SmtpClient smtpClient = new SmtpClient(emailModel.SmtpHost, port);
                smtpClient.UseDefaultCredentials = false;

                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Credentials = new NetworkCredential(emailModel.SmtpEmail, emailModel.SmtpPassword);
                smtpClient.EnableSsl = emailModel.EnableSSL;

                SendEmail(smtpClient, mailMessage);
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError(ex);
            }
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="message">The message.</param>
        private static void SendEmail(SmtpClient client, MailMessage message)
        {
            try
            {
                client.Timeout = 20000;
                client.Send(message);
                foreach (Attachment attachment in message.Attachments)
                {
                    attachment.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.LogError("Error sending email :" + ex.Message);
            }
        }
    }
}
