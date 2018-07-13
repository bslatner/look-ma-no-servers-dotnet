using System;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using Amazon.StepFunctions.Model;
using Newtonsoft.Json;

namespace PreTalkSurvey
{
    public class StepFunctionTasks : IDisposable
    {
        private readonly DataAccess _DataAccess;
        private readonly SmsSender _SmsSender;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public StepFunctionTasks()
        {
            _DataAccess = new DataAccess();
            _SmsSender = new SmsSender();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _DataAccess?.Dispose();
        }

        /// <summary>
        /// Waits for an SMS activity to begin, then handles it.
        /// </summary>
        public async Task WaitSmsAsync(ILambdaContext context)
        {
            var startTime = DateTime.Now;
            var timeAvailable = TimeSpan.FromMinutes(4);
            using (var client = StepFunctionClientFactory.GetClient())
            {
                TimeSpan timeLeft;
                do
                {
                    context.Logger.LogLine("Polling GetActivityTask");
                    var arn = ServerlessConfig.WaitSmsResponseActivityArn;
                    var req = new GetActivityTaskRequest {ActivityArn = arn};
                    var result = await client.GetActivityTaskAsync(req).ConfigureAwait(false);
                    if (result.Input != null && result.TaskToken != null)
                    {
                        context.Logger.LogLine("Received token");

                        // get the current state for the activity
                        var state = JsonConvert.DeserializeObject<State>(result.Input);

                        // save the task token to the survey response and persist the current state
                        // so we can restore it later.
                        var surveyResponse = await _DataAccess.GetSurveyResponseAsync(Sanitizer.HashPhoneNumber(state.PhoneNumber))
                            .ConfigureAwait(false);
                        surveyResponse.TaskToken = result.TaskToken;
                        surveyResponse.SavedState = state;
                        await _DataAccess.SaveSurveyResponeAsync(surveyResponse).ConfigureAwait(false);

                        // ask the question -- do this last so that the current state is persisted with the task token
                        await AskQuestionAsync(state.PhoneNumber, Survey.GetQuestion(state.Question)).ConfigureAwait(false);
                    }

                    timeLeft = timeAvailable - (DateTime.Now - startTime);
                } while (timeLeft.TotalMilliseconds >= 65000);

                context.Logger.LogLine("Polling loop terminated");
            }
        }

        /// <summary>
        /// Start question 1.
        /// </summary>
        public Task<State> Question1Async(State state, ILambdaContext context)
        {
            return QuestionXAsync(1, state, context);
        }

        /// <summary>
        /// Handle error on question 1.
        /// </summary>
        public Task<State> ErrorQuestion1Async(State state, ILambdaContext context)
        {
            return ErrorQuestionXAsync(1, state, context);
        }

        /// <summary>
        /// Start question 2.
        /// </summary>
        public Task<State> Question2Async(State state, ILambdaContext context)
        {
            return QuestionXAsync(2, state, context);
        }

        /// <summary>
        /// Handle error on question 2.
        /// </summary>
        public Task<State> ErrorQuestion2Async(State state, ILambdaContext context)
        {
            return ErrorQuestionXAsync(2, state, context);
        }

        /// <summary>
        /// Start question 3.
        /// </summary>
        public Task<State> Question3Async(State state, ILambdaContext context)
        {
            return QuestionXAsync(3, state, context);
        }

        /// <summary>
        /// Handle error on question 3.
        /// </summary>
        public Task<State> ErrorQuestion3Async(State state, ILambdaContext context)
        {
            return ErrorQuestionXAsync(1, state, context);
        }

        /// <summary>
        /// Start question 4.
        /// </summary>
        public Task<State> Question4Async(State state, ILambdaContext context)
        {
            return QuestionXAsync(4, state, context);
        }

        /// <summary>
        /// Handle error on question 4.
        /// </summary>
        public Task<State> ErrorQuestion4Async(State state, ILambdaContext context)
        {
            return ErrorQuestionXAsync(1, state, context);
        }

        /// <summary>
        /// Ends the survey.
        /// </summary>
        public async Task<State> EndSurveyAsync(State state, ILambdaContext context)
        {
            await SendSms(state.PhoneNumber, "Thank you for participating. Please direct your attention to the devilishly handsome fellow at the podium.").ConfigureAwait(false);
            state.Question = Survey.EndOfQuestions;
            return state;
        }

        private Task<State> QuestionXAsync(int questionNumber, State state, ILambdaContext context)
        {
            var question = Survey.GetQuestion(questionNumber);
            state.Question = question.Id;
            state.IsValidResponse = false;
            return Task.FromResult(state);
        }

        public async Task<State> ErrorQuestionXAsync(int questionNumber, State state, ILambdaContext context)
        {
            await SendSms(state.PhoneNumber, Survey.GetErrorMessage(questionNumber));
            return state;
        } 

        private Task AskQuestionAsync(string phoneNumber, Survey.Question question)
        {
            return SendSms(phoneNumber, question.Text);
        }

        private Task SendSms(string phoneNumber, string body)
        {
            var sms = new SmsMessage
            {
                From = ServerlessConfig.TwilioPhoneNumber,
                To = phoneNumber,
                Body = body,
            };
            return _SmsSender.SendSmsAsync(sms);
        }

    }
}
