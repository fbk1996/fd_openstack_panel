using acl_openstack_identity.Conns;
using System.Net;
using System.Net.Mail;

namespace acl_openstack_identity.Helpers
{
    public class Sender
    {
        private static SmtpClient smtpClient()
        {
            return new SmtpClient()
            {
                Host = connections.smtpHost,
                Port = connections.smtpPort,
                Credentials = new NetworkCredential(connections.smtpUser, connections.smtpPassword),
                EnableSsl = true
            };
        }

        public static async void SendAddUserEmail(string title, string name, string lastname, string organization, string token, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string message = EmailTemplates.AddUserTemplate;
                string messTitle = EmailTemplates.AddUserTitle;

                message = message.Replace("#title", title).Replace("#name", name).Replace("#lastname", lastname).Replace("#organization", organization).Replace("#login", email).Replace("#tokenUrl", $"http://localhost:3000/changePassword/{token}");

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(email);
                    mail.Subject = messTitle;
                    mail.Body = message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                        await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("acl_openstack", "Sender", "SendAddUserEmail", ex);
            }
        }

        public static async void SendAddExistingUserEmail(string title, string name, string lastname, string organization, string email)
        {
            try
            {
                if (string.IsNullOrEmpty(connections.smtpHost) || string.IsNullOrEmpty(connections.smtpUser) || string.IsNullOrEmpty(connections.smtpPassword)) return;

                string message = EmailTemplates.AddExistingUserTemplate;
                string messTitle = EmailTemplates.AddExistingUserTitle;

                message = message.Replace("#title", title).Replace("#name", name).Replace("#lastname", lastname).Replace("#organization", organization).Replace("#login", email);

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(connections.smtpUser);
                    mail.To.Add(email);
                    mail.Subject = messTitle;
                    mail.Body = message;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient())
                        await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                Logger.SendNormalException("acl_openstack", "Sender", "SendAddUserEmail", ex);
            }
        }
    }
}
