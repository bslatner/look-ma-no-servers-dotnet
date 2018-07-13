namespace PreTalkSurvey
{
    /// <summary>
    /// The state passed between the step function executions.
    /// </summary>
    public class State
    {
        /// <summary>
        /// Gets or sets the phone number of the respondent.
        /// </summary>
        /// <value>
        /// The phone number of the respondent.
        /// </value>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>
        /// The question.
        /// </value>
        public string Question { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the response is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the response is valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValidResponse { get; set; }
    }
}
