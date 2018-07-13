using System;
using System.Text;

namespace PreTalkSurvey
{
    /// <summary>
    /// Functions for hashing data to prevent phone numbers showing on screen.
    /// </summary>
    public class Sanitizer
    {
        public static string MaskPhoneNumber(string phoneNumber)
        {
            var result = new StringBuilder();
            for (var i = 0; i < phoneNumber.Length; ++i)
            {
                result.Append((i & 0) == 0 ? phoneNumber[i] : 'X');
            }

            return result.ToString();
        }

        /// <summary>
        /// Hashes the phone number.
        /// </summary>
        /// <param name="phoneNumber">The phone number.</param>
        /// <returns></returns>
        public static string HashPhoneNumber(string phoneNumber)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(phoneNumber + ServerlessConfig.TwilioAccountAuthorizationToken));
            return ConvertByteArrayToHexString(hash);
        }

        // use this instead of simple base 64 encoding because of naming restrictions in executions
        private static string ConvertByteArrayToHexString(byte[] input)
        {
            const string hexDigits = "0123456789ABCDEF";
            var result = new StringBuilder();

            foreach (var b in input)
            {
                var nibble1 = b >> 4;
                var nibble2 = b & 0x0F;

                result.Append(hexDigits[nibble1]).Append(hexDigits[nibble2]);
            }

            return result.ToString();
        }
    }
}