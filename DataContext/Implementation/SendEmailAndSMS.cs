using HelloDoc;
using Services.Contracts;
using Services.Implementation;
using System.Collections;
using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

public class SendEmailAndSMS : ISendEmailAndSMS
{
    private readonly HelloDocDbContext _context;

    public SendEmailAndSMS(HelloDocDbContext context)
    {
        _context = context;
    }

    public void SendSMS()
    {

        Smslog sms = new Smslog
        {
            Mobilenumber = "6351568818",
            Createdate = DateTime.Now,
            Sentdate = DateTime.Now,
            Issmssent = new BitArray(new[] { true }),
            Senttries = 1,
            Smstemplate = "main"
        };

        _context.Add(sms);
        _context.SaveChanges();
       
    }

    public async Task Sendemail(string email, string subject, string message)
    {
        try
        {
            var mail = "tatva.dotnet.yashsarvaiya@outlook.com";
            var password = "Yash@1234";

            var client = new SmtpClient("smtp.office365.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(mail),
                Subject = subject,
                Body = message,
                IsBodyHtml = true // Set to true if your message contains HTML
            };

            mailMessage.To.Add(email);


            Emaillog emaillog = new Emaillog();
            emaillog.Subjectname = subject;
            emaillog.Emailid = email;
            emaillog.Createdate = DateTime.Now;
            emaillog.Sentdate = DateTime.Now;
            emaillog.Isemailsent = new BitArray(new[] { true });
            emaillog.Emailtemplate = "emailtemplate";

            _context.Add(emaillog);
            _context.SaveChanges();

            await client.SendMailAsync(mailMessage);



        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}