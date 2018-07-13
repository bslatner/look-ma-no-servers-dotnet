using System.Collections.Generic;

namespace PreTalkSurvey
{
    /// <summary>
    /// Functions for manipulating HTTP headers.
    /// </summary>
    public static class HeaderHelper
    {
        /// <summary>
        /// Adds the Content-Type header.
        /// </summary>
        public static Dictionary<string, string> AddContentType(this Dictionary<string, string> headers, string contentType)
        {
            headers.Add("Content-Type", contentType);
            return headers;
        }

        /// <summary>
        /// Adds the CORS origin header.
        /// </summary>
        public static Dictionary<string, string> AddCorsOrigin(this Dictionary<string, string> headers)
        {
            headers.Add("Access-Control-Allow-Origin", "*");
            return headers;
        }

        /// <summary>
        /// Adds the CORS headers for the OPTIONS verb.
        /// </summary>
        public static Dictionary<string, string> AddCorsForOptionsVerb(this Dictionary<string, string> headers, IEnumerable<string> allowedMethods)
        {
            headers.AddCorsOrigin();
            headers.Add("X-Requested-With", "*");
            headers.Add("Access-Control-Allow-Headers", "Content-Type,X-Amz-Date,Authorization,X-Api-Key,x-requested-with");
            headers.Add("Access-Control-Allow-Methods", string.Join(",", allowedMethods));
            return headers;
        }

        /// <summary>
        /// Adds the CORS headers for the OPTIONS verb.
        /// </summary>
        public static Dictionary<string, string> AddCorsForOptionsVerb(this Dictionary<string, string> headers, string allowedMethod)
        {
            return AddCorsForOptionsVerb(headers, new[] {allowedMethod});
        }
    }
}
