using Amazon;
using Amazon.StepFunctions;

namespace PreTalkSurvey
{
    /// <summary>
    /// Creates AWS step function clients.
    /// </summary>
    public static class StepFunctionClientFactory
    {
        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <returns></returns>
        public static AmazonStepFunctionsClient GetClient()
        {
            return new AmazonStepFunctionsClient(GetCurrentRegionEndpoint());
        }

        private static RegionEndpoint GetCurrentRegionEndpoint()
        {
            return RegionEndpoint.GetBySystemName(ServerlessConfig.AwsRegion);
        }
    }
}