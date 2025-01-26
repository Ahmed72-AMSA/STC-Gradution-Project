
using Microsoft.Extensions.Options;
using STC.Helpers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace STC.Services{
 public class SMSService:ISMSService{
private readonly TwilioSettings _twilioSettings;


public SMSService(IOptions <TwilioSettings> twilio){
    _twilioSettings = twilio.Value;
    
}

public MessageResource Send(string mobileNumber , string body){
 TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken );
 var result = MessageResource.Create(
 body:body,
 from: new Twilio.Types.PhoneNumber(_twilioSettings.TwilioPhoneNumber),
 to:mobileNumber


 );

 return result;
}


 }
}