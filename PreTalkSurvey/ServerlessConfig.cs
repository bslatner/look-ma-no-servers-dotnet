using System;

namespace PreTalkSurvey
{
    /// <summary>
    /// Configuration values from environment.
    /// </summary>
    public static class ServerlessConfig
    {
        public static string AwsRegion => Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");
        public static string TwilioAccountSid => Environment.GetEnvironmentVariable("TwilioAccountSid");
        public static string TwilioAccountAuthorizationToken => Environment.GetEnvironmentVariable("TwilioAccountAuthorizationToken");
        public static string TwilioPhoneNumber => Environment.GetEnvironmentVariable("TwilioPhoneNumber");
        public static string SurveyTable => Environment.GetEnvironmentVariable("SurveyTable");
        public static string StateMachineArn => Environment.GetEnvironmentVariable("StateMachineArn");
        public static string WaitSmsResponseActivityArn => Environment.GetEnvironmentVariable("WaitSmsResponseActivityArn");
    }
}
