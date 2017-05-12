using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmsAndroidVerification;
using Xunit;

namespace SmsAndroidVerificationTest
{
    public class SmsVerificationHelperTests
    {
        private static readonly Dictionary<string, string> Options = new Dictionary<string, string>
        {
            { "TWILIO_ACCOUNT_SID", "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" },
            { "TWILIO_API_KEY", "SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" },
            { "TWILIO_API_SECRET", "aXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" },
            { "SENDING_PHONE_NUMBER", "+15555555555" },
            { "APP_HASH", "xxxxxx" },
            { "CLIENT_SECRET", "ISXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" }
        };

        private static AppSettings _appSettings;

        public SmsVerificationHelperTests()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(Options)
                .Build();

            _appSettings = new AppSettings();
            config.Bind(_appSettings);
            Microsoft.Extensions.Options.Options.Create(_appSettings);
        }


        private MemoryCache GetCache()
        {
            var cacheOptions = Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions());
            return new MemoryCache(cacheOptions);
        }

        [Fact]
        public void Test_GenerateMessageWithVerificationCode_should_contain_app_hash()
        {
            // Arrange
            var verificationHelper = new SmsVerificationHelper(GetCache(), _appSettings);
            var testNumber = "+17075555555";
            var appHash = _appSettings.APP_HASH;

            // Act
            var message = verificationHelper.GenerateMessageWithVerificationCode(testNumber);
            var verificationCodeRegEx = new Regex(@"[0-9]{6,}", RegexOptions.IgnoreCase);

            // Assert
            Assert.True(message.IndexOf(appHash, StringComparison.Ordinal) > -1);
            Assert.True(verificationCodeRegEx.IsMatch(message));
        }

        [Fact]
        public void Test_VerifyCode()
        {
            // Arrange
            var verificationHelper = new SmsVerificationHelper(GetCache(), _appSettings);
            var testNumber = "+17075555555";

            // Act
            var verificationCodeMsg = verificationHelper.GenerateMessageWithVerificationCode(testNumber);
            var verificationCodeRegEx = new Regex(@"[0-9]{6,}", RegexOptions.IgnoreCase);
            var verificationCode = verificationCodeRegEx.Match(verificationCodeMsg).Value;

            // Assert
            Assert.True(verificationHelper.VerifyCode(testNumber, verificationCode));
        }

        [Fact]
        public void Test_ResetVerificationCodeForNumber()
        {
            // Arrange
            var verificationHelper = new SmsVerificationHelper(GetCache(), _appSettings);
            var testNumber = "+17075555555";
            var verificationCodeMsg = verificationHelper.GenerateMessageWithVerificationCode(testNumber);
            var verificationCodeRegEx = new Regex(@"[0-9]{6,}", RegexOptions.IgnoreCase);
            var verificationCode = verificationCodeRegEx.Match(verificationCodeMsg).Value;
            Assert.True(verificationHelper.VerifyCode(testNumber, verificationCode));

            // Act
            verificationHelper.ResetVerificationCodeForNumber(testNumber);

            // Assert
            Assert.False(verificationHelper.VerifyCode(testNumber, verificationCode));
        }
    }

}
