using System;
using System.Collections.Generic;
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
}