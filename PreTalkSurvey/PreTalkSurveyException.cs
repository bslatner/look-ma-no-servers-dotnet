using System;
using System.Runtime.Serialization;

namespace PreTalkSurvey
{
    [Serializable]
    public class PreTalkSurveyException : Exception
    {
        public PreTalkSurveyException()
        {
        }

        public PreTalkSurveyException(string message) : base(message)
        {
        }

        public PreTalkSurveyException(string message, Exception inner) : base(message, inner)
        {
        }

        protected PreTalkSurveyException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}