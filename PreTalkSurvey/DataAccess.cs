using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace PreTalkSurvey
{
    /// <summary>
    /// Data access functions.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class DataAccess : IDisposable
    {
        private readonly AmazonDynamoDBClient _Client;
        private readonly DynamoDBContext _Context;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAccess"/> class.
        /// </summary>
        public DataAccess()
        {
            var tableName = ServerlessConfig.SurveyTable;
            if (!string.IsNullOrEmpty(tableName))
            {
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(SurveyResponse)] = 
                    new Amazon.Util.TypeMapping(typeof(SurveyResponse), tableName);
            }

            var config = new DynamoDBContextConfig { 
                Conversion = DynamoDBEntryConversion.V2,
                ConsistentRead = true
            };
            _Client = new AmazonDynamoDBClient();
            _Context = new DynamoDBContext(_Client, config);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _Client?.Dispose();
            _Context?.Dispose();
        }

        /// <summary>
        /// Gets the survey response.
        /// </summary>
        /// <param name="phoneNumberHash">The hash of the phone number.</param>
        /// <returns></returns>
        public Task<SurveyResponse> GetSurveyResponseAsync(string phoneNumberHash)
        {
            return _Context.LoadAsync<SurveyResponse>(phoneNumberHash);
        }

        /// <summary>
        /// Saves the survey respone.
        /// </summary>
        /// <param name="surveyResponse">The survey response.</param>
        /// <returns></returns>
        public Task SaveSurveyResponeAsync(SurveyResponse surveyResponse)
        {
            return _Context.SaveAsync(surveyResponse);
        }

        public async Task<List<SurveyResponse>> GetAllResponsesAsync()
        {
            var result = new List<SurveyResponse>();
            var search = _Context.ScanAsync<SurveyResponse>(Enumerable.Empty<ScanCondition>());
            do
            {
                var reponseList = await search.GetNextSetAsync().ConfigureAwait(false);
                result.AddRange(reponseList);
            } while (!search.IsDone);

            return result;
        }
    }
}
