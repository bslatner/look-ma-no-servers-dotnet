using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.StepFunctions.Model;
using Newtonsoft.Json;

namespace PreTalkSurvey
{
    /// <summary>
    /// Handler for Twilio messages.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TwilioHandler : IDisposable
    {
        private readonly DataAccess _DataAccess;
        private readonly SmsSender _SmsSender;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public TwilioHandler()
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

        private static NameValueCollection GetFormValues(APIGatewayProxyRequest request)
        {
            return System.Web.HttpUtility.ParseQueryString(request.Body);
        }

        //Useful for debugging, not needed otherwise
        //private static void LogRequest(APIGatewayProxyRequest request, ILambdaContext context)
        //{
        //    var data = JsonConvert.SerializeObject(request);
        //    context.Logger.LogLine("Request body:");
        //    context.Logger.LogLine(data);
        //    if (request.Headers["Content-Type"] == "application/x-www-form-urlencoded")
        //    {
        //        var formValues = GetFormValues(request);
        //        foreach (var b in formValues.AllKeys)
        //        {
        //            context.Logger.LogLine($"{b}: {formValues[b]}");
        //        }
        //    }
        //}

        private static APIGatewayProxyResponse GetOkResponse()
        {
            return new APIGatewayProxyResponse
            {
                StatusCode = (int)HttpStatusCode.OK,
                Body = "<Response />",
                Headers = new Dictionary<string, string> { { "Content-Type", "text/xml" } }
            };
        }

        private static SmsMessage GetSmsMessage(NameValueCollection formValues)
        {
            return new SmsMessage
            {
                From = formValues["From"],
                To = formValues["To"],
                Body = formValues["Body"]
            };
        }

        private static bool EqualsIgnoreCase(string x, string y)
        {
            var x2 = x.Trim();
            var y2 = y.Trim();
            return string.Equals(x2, y2, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Receive an SMS message.
        /// </summary>
        public async Task<APIGatewayProxyResponse> ReceiveSmsAsync(APIGatewayProxyRequest request, ILambdaContext context)
        {
            var logger = context.Logger;

            logger.LogLine("Receiving SMS");

            // API Gateway isn't nice like MVC. We need to parse the form ourselves.
            var formValues = GetFormValues(request);
            if (!TwilioRequestValidator.IsValidPostRequest(request, formValues))
            {
                throw new PreTalkSurveyException("Unable to validate signature on request");
            }

            // Get the SMS message from the form and make sure it comes from the expected phone number.
            var sms = GetSmsMessage(formValues);
            if (sms.To != ServerlessConfig.TwilioPhoneNumber)
            {
                throw new PreTalkSurveyException("Invalid incoming phone number");
            }

            // figure out what to do with the incoming message.
            try
            {
                var surveyResponse = await _DataAccess.GetSurveyResponseAsync(Sanitizer.HashPhoneNumber(sms.From)).ConfigureAwait(false);
                if (surveyResponse == null)
                {
                    // if we've never seen this phone number before, it's the start of a new survey
                    logger.LogLine($"New phone number {Sanitizer.MaskPhoneNumber(sms.From)}, starting new survey");
                    await StartNewSurveyAsync(sms.From, logger).ConfigureAwait(false);
                }

                else if (!string.IsNullOrEmpty(surveyResponse.TaskToken))
                {
                    if (EqualsIgnoreCase(sms.Body, "hello") || EqualsIgnoreCase(sms.Body, "start"))
                    {
                        // if they send "hello" or "start", terminate the workflow and start it again
                        logger.LogLine($"Restarting workflow for {Sanitizer.MaskPhoneNumber(sms.From)}");
                        await FailWaitForResponseAsync(surveyResponse.TaskToken).ConfigureAwait(false);
                        await StartNewSurveyAsync(sms.From, logger).ConfigureAwait(false);
                    }

                    // if there's a task token, we need to advance the state machine
                    var taskToken = surveyResponse.TaskToken;
                    var state = surveyResponse.SavedState;

                    // evaluate the answer to the question
                    var questionResponse = Survey.EvaluateResponseToQuestion(state.Question, sms.Body.Trim());
                    state.IsValidResponse = questionResponse.IsValidResponse;
                    logger.LogLine($"Evaluated response to question \"{state.Question}\". Response \"{sms.Body}\" from {Sanitizer.MaskPhoneNumber(sms.From)}. IsValidResponse={questionResponse.IsValidResponse};StandardizedResponse={questionResponse.StandardizedResponse}");

                    // clear the token and the saved state -- we want to make sure these values aren't reused
                    // so we save before completing the activity.
                    logger.LogLine($"Clearing state for {Sanitizer.MaskPhoneNumber(sms.From)}");
                    surveyResponse.TaskToken = "";
                    surveyResponse.SavedState = null;

                    // store the answer
                    if (state.IsValidResponse)
                    {
                        surveyResponse.Responses[questionResponse.Question] = questionResponse.StandardizedResponse;
                    }
                    await _DataAccess.SaveSurveyResponeAsync(surveyResponse);

                    // store the answer to the question

                    // advance the state machine
                    logger.LogLine($"Completing response for task token {taskToken}");
                    await CompleteWaitForResponseAsync(taskToken, state).ConfigureAwait(false);
                }

                else
                {
                    // they must have previously completed the survey - start again
                    logger.LogLine($"Restarting workflow for {Sanitizer.MaskPhoneNumber(sms.From)} - previously completed");
                    await StartNewSurveyAsync(sms.From, logger).ConfigureAwait(false);
                }
            }
            catch
            {
                logger.LogLine($"Sending snarky error message to {Sanitizer.MaskPhoneNumber(sms.From)}");
                await SendErrorMessage(sms.From).ConfigureAwait(false);
                throw;
            }

            logger.LogLine("Returning OK response");
            return GetOkResponse();
        }

        private async Task StartNewSurveyAsync(string phoneNumber, ILambdaLogger logger)
        {
            // save info about the respondent
            logger.LogLine($"Saving survey response for {Sanitizer.MaskPhoneNumber(phoneNumber)}");
            var phoneNumberHash = Sanitizer.HashPhoneNumber(phoneNumber);
            var surveyResponse = new SurveyResponse
            {
                PhoneNumberHash = phoneNumberHash,
                TaskToken = "",
                Responses = new Dictionary<string, string>()
            };
            await _DataAccess.SaveSurveyResponeAsync(surveyResponse).ConfigureAwait(false);

            // start the workflow
            logger.LogLine($"Starting workflow for {Sanitizer.MaskPhoneNumber(phoneNumber)}");
            using (var client = StepFunctionClientFactory.GetClient())
            {
                var state = new State
                {
                    PhoneNumber = phoneNumber

                };
                var req = new StartExecutionRequest
                {
                    Name = phoneNumberHash + Guid.NewGuid().ToString("N"),
                    StateMachineArn = ServerlessConfig.StateMachineArn,
                    Input = JsonConvert.SerializeObject(state)
                };
                await client.StartExecutionAsync(req).ConfigureAwait(false);
            }
        }

        private static async Task CompleteWaitForResponseAsync(string taskToken, State state)
        {
            using (var client = StepFunctionClientFactory.GetClient())
            {
                var req = new SendTaskSuccessRequest
                {
                    TaskToken = taskToken,
                    Output = JsonConvert.SerializeObject(state)
                };
                await client.SendTaskSuccessAsync(req).ConfigureAwait(false);
            }
        }

        private static async Task FailWaitForResponseAsync(string taskToken)
        {
            using (var client = StepFunctionClientFactory.GetClient())
            {
                var req = new SendTaskFailureRequest
                {
                    TaskToken = taskToken, 
                    Cause = "User requested do-over",
                    Error = "Restart"                    
                };
                await client.SendTaskFailureAsync(req).ConfigureAwait(false);
            }
        }

        public Task SendErrorMessage(string phoneNumber)
        {
            var sms = new SmsMessage
            {
                From = ServerlessConfig.TwilioPhoneNumber,
                To = phoneNumber,
                Body = "Something went wrong. Please heckle the presenter."
            };
            return _SmsSender.SendSmsAsync(sms);
        }
    }
}
