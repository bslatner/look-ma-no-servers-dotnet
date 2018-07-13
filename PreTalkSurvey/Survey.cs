using System;
using System.Collections.Generic;
using System.Linq;

namespace PreTalkSurvey
{
    public static class Survey
    {
        public const string EndOfQuestions = "END";

        public enum ResponseType
        {
            Numeric1To5,
            YesNo
        }

        public class Question
        {
            public string Id { get; set; }
            public string Text { get; set; }
            public ResponseType ResponseType { get; set; }

            public Question(string id, string text, ResponseType responseType)
            {
                Id = id;
                Text = text;
                ResponseType = responseType;
            }
        }

        public static List<Question> Questions = new List<Question>
        {
            new Question("skill-dotnet", "What is your skill level with .NET? Respond 1 to 5, with 1 being \"I've never used .NET\" and 5 being \"I'm a .NET ninja\"", ResponseType.Numeric1To5),
            new Question("familiar-aws", "How familiar are you with AWS? Respond 1 to 5, with 1 being \"not at all familiar\" and 5 being \"I'm an AWS expert\"", ResponseType.Numeric1To5),
            new Question("familiar-aws-serverless", "How familiar are you with AWS's serverless technologies? Respond 1 to 5, with 1 being \"not at all familiar\" and 5 being \"I eat Lambda function for breakfast\"", ResponseType.Numeric1To5),
            new Question("understand-aws-security", "Do you understand how AWS IAM users, groups, roles, and policies work? Respond yes or no (or true/false or 1/0 if you're hardcore)", ResponseType.YesNo)
        };

        public class Response
        {
            public string Question { get; set; }
            public bool IsValidResponse { get; set; }
            public string StandardizedResponse { get; set; }
        }

        public static Question GetQuestion(string id)
        {
            return Questions.Single(q => q.Id == id);
        }

        public static Question GetQuestion(int questionNumber)
        {
            return Questions[questionNumber - 1];
        }

        public static string GetErrorMessage(int questionNumber)
        {
            switch (GetQuestion(questionNumber).ResponseType)
            {
                case ResponseType.Numeric1To5:
                    return "Please respond with a number between 1 and 5.";

                case ResponseType.YesNo:
                    return "Please respond \"yes\" or \"no\".";

                default:
                    throw new InvalidOperationException($"Unknown ResponseType for question {questionNumber}.");
            }
        }

        private static bool EqualsIgnoreCase(string x, string y)
        {
            var x2 = x.Trim();
            var y2 = y.Trim();
            return string.Equals(x2, y2, StringComparison.OrdinalIgnoreCase);
        }

        public static Response EvaluateResponseToQuestion(string id, string response)
        {
            // evaluate response that should be 1-5
            var question = GetQuestion(id);
            if (question.ResponseType == ResponseType.Numeric1To5)
            {
                if (int.TryParse(response, out var intValue))
                {
                    return new Response
                    {
                        Question = question.Id,
                        IsValidResponse = true,
                        StandardizedResponse = intValue.ToString()
                    };
                }

                return new Response
                {
                    Question = question.Id,
                    IsValidResponse = false
                };
            }

            // evaluate yes/no response
            if (question.ResponseType == ResponseType.YesNo)
            {
                var isYes = EqualsIgnoreCase(response, "yes") ||
                            EqualsIgnoreCase(response, "true") ||
                            EqualsIgnoreCase(response, "1");
                var isNo  = EqualsIgnoreCase(response, "no") ||
                            EqualsIgnoreCase(response, "false") ||
                            EqualsIgnoreCase(response, "0");
                if (isYes || isNo)
                {
                    return new Response
                    {
                        Question = question.Id,
                        IsValidResponse = true,
                        StandardizedResponse = isYes.ToString()
                    };
                }

                return new Response
                {
                    Question = question.Id,
                    IsValidResponse = false
                };
            }

            throw new InvalidOperationException($"Unknown ResponseType for question {id}.");
        }

        public static IEnumerable<ReportResult> GetReport(List<SurveyResponse> responses)
        {
            return 
                // group responses by their question
                from response in responses
                from indidivualResponse in response.Responses
                group indidivualResponse by indidivualResponse.Key
                into questionResponses
                select new ReportResult
                {
                    Question = questionResponses.Key,

                    // group answers to questions and compute percentages
                    Items = (from qr in questionResponses
                        group qr by qr.Value
                        into answer
                        select new ReportItem
                        {
                            Answer = answer.Key,
                            Percentage = 100.0 * answer.Count() / questionResponses.Count()
                        }).ToArray()
                };
        }
    }
}
