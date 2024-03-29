using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

public static class SendEmailAndSMS
{
    public static void SendSMS()
    {
        var accountSid = "AC3536fafe53afa4ff18883525e84a0acd";
        var authToken = "2294f0d25917b127344576878688e200";
        TwilioClient.Init(accountSid, authToken);

        var message = MessageResource.Create(
           body: "yash sarvaiya",
           from: new Twilio.Types.PhoneNumber("+19287560075"),
           to: new Twilio.Types.PhoneNumber("+916351568818")
       );

    }

    public static async Task Sendemail(string email, string subject, string message)
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

           

            await client.SendMailAsync(mailMessage);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}