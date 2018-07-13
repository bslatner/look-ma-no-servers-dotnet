# Look Ma, No Servers: AWS Serverless Applications with the .NET Stack

Thanks for attending my talk on doing fun serverless things with .NET on AWS.

If you're using Visual Studio, you should
[install the Visual Studio tools](https://aws.amazon.com/visualstudio/) first.

Here's how to run the demos.

## General Stuff

First, you need to create an AWS Profile if you haven't done that already. You can do that
from the AWS Explorer in Visual Studio, or with the PowerShell tools.

```powershell
Set-AWSCredential -AccessKey AKIAIOSFODNN7EXAMPLE -SecretKey wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY -StoreAs MyProfileName
```

More info on profiles [can be found on amazon.com](https://docs.aws.amazon.com/powershell/latest/userguide/specifying-your-aws-credentials.html).

Second, you need to create an S3 bucket where all this serverless code will be deployed.
Again, you can do this from AWS Explorer or from PowerShell:

```powershell
New-S3Bucket -BucketName my-new-bucket -Region us-east-1
```

Third, you need to create a role called `look-ma-basic-execution` for the HelloWorld
simple example.

To do this, log into the AWS Console with your browser, navigate to IAM, click on Roles,
and create a new role. When asked what service the role is for, choose Lambda.
When asked to apply policies to the role, choose the AWS policy called `AWSLambdaBasicExecutionRole`.
Make sure you name the role `look-ma-basic-execution`. 

Finally, most of the demos have a file called aws-lambda-tools-defaults.json. You
need to replace the profile name and bucket name in those files with your own profile name
and bucket name. There is also an `appSettings.json` file in `HelloWorldClient` 
where you should replace the profile name.

Now we can get started!

## HelloWorld/HelloWorldClient

This demo creates a simple, single Lambda function. The client app executes it and 
demonstrates that it works.

First, deploy the `HelloWorld` demo. You can right click on it in Visual Studio and
choose Publish to AWS Lambda. Or, from the command line, navigate to the `HelloWorld`
directory and type:

```batch
dotnet restore
dotnet lambda deploy-function
```

The initial `dotnet restore` is required to download the AWS Lambda tooling package.

Once the function is deployed, you can run the HelloWorldClient application, either by
running it in Visual Studio or from the command line with `dotnet run`.

If you've done everything correctly, the client should print:

```
Lambda Response:
"Hello, Bryan!"
```

Feel free to change the code to use another name.

## HelloWorldRestApi

This demo creates a simple, single Lambda function that is invoked by an API Gateway
trigger.

First, deploy the `HelloWorldRestApi` demo. You can right click on it in Visual Studio and
choose Publish to AWS Lambda. Or, from the command line, navigate to the `HelloWorldRestApi`
directory and type:

```batch
dotnet restore
dotnet lambda deploy-serverless
```

The initial `dotnet restore` is required to download the AWS Lambda tooling package.

The deployment should give you a URL for your new rest API. Open a browser and navigate
to the API URL. The full URL should look something like:

```
https://xxxxxxxxxx.execute-api.us-east-1.amazonaws.com/Prod/hello/YourName
```

If all goes well, you should see the following in your browser:

```json
{"Greeting":"Hello, YourName!"}
```

## HelloWorldAspNet

This project demonstrates how to deploy an ASP.NET Razor Pages application
using API Gateway.

First, deploy the `HelloWorldAspNet` demo. You can right click on it in Visual Studio and
choose Publish to AWS Lambda. Or, from the command line, navigate to the `HelloWorldAspNet`
directory and type:

```batch
dotnet restore
dotnet lambda deploy-serverless
```

The initial `dotnet restore` is required to download the AWS Lambda tooling package.

The deployment should give you a URL for your application. Open a browser and navigate
to the URL. The full URL should look something like:

```
https://xxxxxxxxxx.execute-api.us-east-1.amazonaws.com/Prod
```

If all goes well, the home page of the web application should render.


## HelloWorldStepFunction

First, deploy the `HelloWorldStepFunction` demo. You can right click on it in Visual Studio and
choose Publish to AWS Lambda. Or, from the command line, navigate to the `HelloWorldStepFunction`
directory and type:

```batch
dotnet restore
dotnet lambda deploy-serverless
```

The initial `dotnet restore` is required to download the AWS Lambda tooling package.

After the project is deployed, log into the AWS console with your browser and go to
Step Functions. Find your new state machine and click on it. From the state machine view,
click Start Execution. When prompted for the state machine input, provide some JSON:

```json
{
    "Name": "YourName",
    "Language": "English"
}
```

then click Start Execution.

If all goes well, you should be shown a visualization of the workflow. The tasks that
execute successfully should turn green. Click on the individual nodes in the diagram to see
the inputs and outputs. If all is successful, when you click on the EnglishGreeting node,
you should see the following output:

```json
{
  "Name": "YourName",
  "Greeting": "Hello, YourName!"
}
```

## PreTalkSurvey

If you want your own version of the pre-talk survey I did with text messaging, 
you first need to create an account with [Twilio](https://twilio.com). Once your
account is created, you need to buy a phone number. Make note of that number.

Next, in the parent directory of the LookMaNoServers solution, create a batch file
called `build-secrets.cmd`. So, if you have the `.sln` file in
`C:\Development\ServerlessDemos`, you would create this file in
`C:\Development`. It should look like this:

```batch
@ECHO OFF
SET TwilioAccountSid=**YourAccountSid**
SET TwilioAccountAuthorizationToken=**YourAuthorizationToken**
SET TwilioPhoneNumber=**TheNumberYouBought**
```

You can get your Account SID and Authorization token from the Twilio dashboard. It's very 
important to get the phone number format correct. For US phone numbers, it should look 
like +1XXXYYYZZZZ. If you don't do this correctly, the demo won't work.

Once you've created the build secrets file, open a command prompt and navigate to the 
directory where the `.sln` file is. Run the `build-pre-talk-survey.cmd` batch file.
If all goes well, you'll get a URL from the deployment like you did with the
`HelloWorldRestApi` demo. 

Open the Twilio dashboard and find your phone number. Click on it. Under the Messaging
section of the number configuration, choose "WebHooks" for the Configure With field.
In the A Message Comes In field, choose "WebHook" and then paste the URL of your API.
Make sure "HTTP POST" is chosen as the WebHook method. Your full URL should look something
like this:

```
https://xxxxxxxxxx.execute-api.us-east-1.amazonaws.com/Prod/receive-sms
```

Click Save to update the number's configuration.

If you've done everything correctly, you should be able to run the demo by sending a text
message to your Twilio phone number.
