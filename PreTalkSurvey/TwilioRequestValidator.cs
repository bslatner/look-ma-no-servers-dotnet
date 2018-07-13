using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Amazon.Lambda.APIGatewayEvents;

namespace PreTalkSurvey
{
    /// <summary>
    /// Validates requests coming from Twlio.
    /// </summary>
    public static class TwilioRequestValidator
    {

        /// <summary>
        /// Determines whether the request from Twilio is valid. See http://www.twilio.com/docs/security-reliability/security
        /// </summary>
        /// <param name="request">The API gateway proxy request.</param>
        /// <param name="formValues">The form values.</param>
        /// <returns>
        ///   <c>true</c> if if the request is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidPostRequest(APIGatewayProxyRequest request, NameValueCollection formValues)
        {
            var fullUrl = $"{request.Headers["CloudFront-Forwarded-Proto"]}://{request.Headers["Host"]}{request.RequestContext.Path}";
            return IsValidPostRequest(fullUrl, formValues, request.Headers["X-Twilio-Signature"]);
        }

        /// <summary>
        /// Determines whether the request from Twilio is valid. See http://www.twilio.com/docs/security-reliability/security
        /// </summary>
        /// <param name="fullUrl">The full URL of the request, from the protocol (http...) through the end of the query string (everything after the ?).</param>
        /// <param name="formValues">The form values.</param>
        /// <param name="twilioSignature">The signature Twilio sent with th request.</param>
        /// <returns>
        ///   <c>true</c> if if the request is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidPostRequest(string fullUrl, NameValueCollection formValues, string twilioSignature)
        {
            var expectedSig = GetExpectedSignatureFromRequest(fullUrl, formValues);

            return expectedSig == twilioSignature;
        }

        private static string GetExpectedSignatureFromRequest(string fullUrl, NameValueCollection formValues)
        {
            var value = new StringBuilder(fullUrl);

            // Iterate through that sorted list of form parameters, and append the variable name and value (with no delimiters)
            // to the end of the URL string.
            var sortedKeys = formValues.AllKeys.OrderBy(k => k, StringComparer.Ordinal).ToList();
            foreach (var key in sortedKeys)
            {
                value.Append(key);
                value.Append(formValues[key]);
            }

            // Sign the resulting value with HMAC-SHA1 using the AuthToken as the key.
            var sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(ServerlessConfig.TwilioAccountAuthorizationToken));
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(value.ToString()));

            // Base64 encode the hash
            return Convert.ToBase64String(hash);
        }
    }
}
