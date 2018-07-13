using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;

namespace PreTalkSurvey
{
    /// <summary>
    /// Handles reporting API calls.
    /// </summary>
    public class ReportHandler : ApiHandlerBase
    {
        private readonly DataAccess _DataAccess;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportHandler"/> class.
        /// </summary>
        public ReportHandler()
        {
            _DataAccess = new DataAccess();
        }

        /// <summary>
        /// Returns the JSON used by the report pie charts.
        /// </summary>
        public async Task<APIGatewayProxyResponse> ReportAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var allResults = await _DataAccess.GetAllResponsesAsync().ConfigureAwait(false);
            var report = Survey.GetReport(allResults);
            return GetJsonResponse(report);
        }

        /// <summary>
        /// Returns options for report.
        /// </summary>
        public Task<APIGatewayProxyResponse> ReportOptionsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            return Task.FromResult(GetGetOptions());
        }
    }
}
