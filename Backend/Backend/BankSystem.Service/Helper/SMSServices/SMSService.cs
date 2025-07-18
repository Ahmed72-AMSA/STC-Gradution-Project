using BankSystem.Data.Entities.Helpers;
using BankSystem.Service.Helper.SMSServices;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace BankSystem.Service.Helper.SMSService
{
    public class SMSService : ISMSService
    {
        private readonly TwilioSettings _twilioSettings;


        public SMSService(IOptions<TwilioSettings> twilio)
        {
            _twilioSettings = twilio.Value;

        }

        public MessageResource Send(string mobileNumber, string body)
        {
            TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);
            var result = MessageResource.Create(
            body: body,
            from: new Twilio.Types.PhoneNumber(_twilioSettings.TwilioPhoneNumber),
            to: mobileNumber


            );

            return result;
        }


    }
}
