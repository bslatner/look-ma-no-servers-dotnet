using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Microsoft.Extensions.Configuration;

namespace HelloWorldClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");

            var configuration = builder.Build();

            var awsOptions = configuration.GetAWSOptions();

            var lambdaClient = awsOptions.CreateServiceClient<IAmazonLambda>();

            var request = new InvokeRequest
            {
                FunctionName = "LookMaHelloWorld",
                Payload = "{ \"Name\": \"Bryan\" }"
            };
            var response = await lambdaClient.InvokeAsync(request);

            var reader = new StreamReader(response.Payload);
            Console.WriteLine("Lambda Response:");
            Console.WriteLine(await reader.ReadToEndAsync());
        }
    }
}
