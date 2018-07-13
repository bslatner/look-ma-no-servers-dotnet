using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;

namespace PreTalkSurvey
{
    /// <summary>
    /// Base class for handler classes that use API Gateway proxy.
    /// </summary>
    public abstract class ApiHandlerBase
    {
        protected static APIGatewayProxyResponse GetJsonResponse<T>(T data)
        {
            return GetJsonResponseWithHttpStatus(data, HttpStatusCode.OK);
        }

        protected static APIGatewayProxyResponse GetJsonResponseWithHttpStatus<T>(T data, HttpStatusCode httpStatusCode)
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)httpStatusCode,
                Body = JsonConvert.SerializeObject(data),
                Headers = GetJsonHeaders()
            };
        }

        protected static Dictionary<string, string> GetJsonHeaders()
        {
            return new Dictionary<string, string>()
                .AddContentType("application/json")
                .AddCorsOrigin();
        }

        protected static APIGatewayProxyResponse GetGetOptions()
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Headers = new Dictionary<string, string>().AddCorsForOptionsVerb("GET")
            };
        }
    }
}
