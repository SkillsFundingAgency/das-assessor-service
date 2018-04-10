using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace SFA.DAS.AssessorService.PrintFunctionProcessFlow.EMail
{
    public class EMailSender
    {
        public const string MAIL_HOST = "localhost";
        public const int MAIL_PORT = 1025;

        public async Task SendEMail()
        {
            var email = "test@fake.com";

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Generator", "generator@generator.com"));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = "Here are your random names";
            message.Body = new TextPart("plain")
            {
                Text = string.Join(Environment.NewLine, Names())
            };

            using (var mailClient = new SmtpClient())
            {
                await mailClient.ConnectAsync(MAIL_HOST, MAIL_PORT, SecureSocketOptions.None);
                await mailClient.SendAsync(message);
                await mailClient.DisconnectAsync(true);
            }
        }

        public IEnumerable<string> Names()
        {
            return new List<string>
            {
                "Alan Burns",
                "John Coxhead",
                "David Gouge"
            };
        }
    }
}
