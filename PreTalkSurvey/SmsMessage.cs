namespace PreTalkSurvey
{
    /// <summary>
    /// An SMS message.
    /// </summary>
    public class SmsMessage
    {
        /// <summary>
        /// Gets or sets the phone number the message came/comes from.
        /// </summary>
        /// <value>
        /// The phone number the messages came/comes from.
        /// </value>
        public string From { get; set; }

        /// <summary>
        /// Gets or sets the phone number the messages goes/went to.
        /// </summary>
        /// <value>
        /// The phone number the messages goes/went to.
        /// </value>
        public string To { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        public string Body { get; set; }
    }
}