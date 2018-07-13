using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

namespace PreTalkSurvey
{
    /// <summary>
    /// Response to a survey.
    /// </summary>
    [DynamoDBTable("pre-talk-survey")]
    public class SurveyResponse
    {
        /// <summary>
        /// Gets or sets the hash of the phone number of the respondent.
        /// </summary>
        /// <value>
        /// The hash of the phone number of the respondent.
        /// </value>
        [DynamoDBHashKey]
        public string PhoneNumberHash { get; set; }

        /// <summary>
        /// Gets or sets the task token.
        /// </summary>
        /// <value>
        /// The task token for the step function activity.
        /// </value>
        public string TaskToken { get; set; }

        /// <summary>
        /// Gets or sets the saved state.
        /// </summary>
        /// <value>
        /// The saved state.
        /// </value>
        public State SavedState { get; set; }

        /// <summary>
        /// Gets or sets the responses.
        /// </summary>
        /// <value>
        /// The responses.
        /// </value>
        public Dictionary<string,string> Responses { get; set; }
    }
}