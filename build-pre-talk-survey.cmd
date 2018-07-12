@echo off
call ..\build-secrets.cmd
cd PreTalkSurvey
dotnet lambda deploy-serverless -tp TwilioAccountSid=%TwilioAccountSid%;TwilioAccountAuthorizationToken=%TwilioAccountAuthorizationToken%;TwilioPhoneNumber=%TwilioPhoneNumber%
cd ..