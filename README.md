<a href="https://www.twilio.com">
  <img src="https://static0.twilio.com/marketing/bundles/marketing/img/logos/wordmark-red.svg" alt="Twilio" width="250" />
</a>

# Archived - SMS Verification for Android for C Sharp

Please visit the [App Verification with SMS](https://www.twilio.com/docs/sms/app-verification) page on the Twilio Docs to learn more about how to use App Verification for Android with Twilio SMS.

## Description

This sample project demonstrates how to use Twilio's SMS Messaging APIs
to verify Android application user's phone numbers. This application supports the SMS Verifier APIs
from Google Play Services 

You'll also need to have a sample Android application that you configure to use these URLs to verify phone
numbers. You can download that application from Google here 

## Setup .NET Core SDK (If you don't have Visual Studio 2015/2017)

1. Install [.NET Core](https://www.microsoft.com/net/core).

### Clone and Restore Project Dependencies
```bash
git clone git@github.com:TwilioDevEd/sms-verification-android-csharp.git
cd sms-verification-android-csharp
dotnet restore
```

## Configure the sample application

To run the application, you'll need to gather your Twilio account credentials and configure them
in a file named `appsettings.json`. Specifically you must update the section titled "AppSettings"
which looks something like this:

```json
  "AppSettings": {
    "TWILIO_ACCOUNT_SID": "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
    "TWILIO_API_KEY": "SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
    "TWILIO_API_SECRET": "aXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
    "SENDING_PHONE_NUMBER": "+15555555555",
    "APP_HASH": "XXXXXXX",
    "CLIENT_SECRET": "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX"
  },
```

Open `appsettings.json` in your favorite text editor and configure the following values. You will need all of these values before you continue.

| Config Value  | Description |
| :-------------  |:------------- |
`TWILIO_ACCOUNT_SID` | Your primary Twilio account identifier - find this [in the console here](https://www.twilio.com/console).
`TWILIO_API_KEY` | Used to authenticate - [generate one here](https://www.twilio.com/console/dev-tools/api-keys).
`TWILIO_API_SECRET` | Used to authenticate - [just like the above, you'll get one here](https://www.twilio.com/console/dev-tools/api-keys).
`SENDING_PHONE_NUMBER` | This phone number will be sending the SMS messages to the Android device. Either use a phone number you purchased through Twilio, or one you have verified with your account.
`APP_HASH` | Matches the Android application to the SMS message - get this from the Settings menu item in the Android application.
`CLIENT_SECRET` | Matches the Android application to the server - you can set this in the `strings.xml` file in your Android application. It must match the server's config value. You can also override it in the Settings menu of the app.

## Run the sample application

### Visual Studio

Open `SmsAndroidVerification.sln` and hit *F5*

### OS X or Linux
```bash
cd SmsAndroidVerification
dotnet run
```

Your application should now be running at [http://localhost:5000/](http://localhost:5000/).

Check your config values, and then make sure everything looks good.

## Running the server with ngrok

Your phone won't be able to access localhost directly. You'll need to create a publicly accessible URL
using a tool like [ngrok](https://ngrok.com/) to send HTTP/HTTPS traffic to a server running on your localhost.

You can also deploy this application to a server, or to a cloud provider such as Google App Engine.

```bash
ngrok http 5000
```

## Setting the server for the Android application

You'll need to update the Android application with the URLs from ngrok. Open up the `strings.xml` file and add these lines:

``` xml
    <string name="url_request">https://YOUR_SERVER_HERE/api/request</string>
    <string name="url_verify">https://YOUR_SERVER_HERE/api/verify</string>
    <string name="url_reset">https://YOUR_SERVER_HERE/api/reset</string>
```

Replace YOUR_SERVER_HERE with your server's host name or IP address.

## License
MIT
