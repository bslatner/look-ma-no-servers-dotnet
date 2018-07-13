using System.Threading.Tasks;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace PreTalkSurvey
{
    /// <summary>
    /// Sends SMS messages using Twilio.
    /// </summary>
    public class SmsSender
    {
        private static TwilioRestClient GetClient()
        {
            return new TwilioRestClient(ServerlessConfig.TwilioAccountSid, ServerlessConfig.TwilioAccountAuthorizationToken);
        }

        /// <summary>
        /// Sends an SMS message.
        /// </summary>
        /// <param name="sms">The SMS.</param>
        public async Task SendSmsAsync(SmsMessage sms)
        {
            var client = GetClient();
            await MessageResource.CreateAsync(
                from: new PhoneNumber(sms.From), 
                to: new PhoneNumber(sms.To),
                body: sms.Body,
                client: client).ConfigureAwait(false);
        }
    }
}
