using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SmsAndroidVerification;
using SmsAndroidVerification.Controllers;
using Xunit;

namespace SmsAndroidVerificationTest
{
    public class SmsVerificationControllerTest
    {
        private static readonly Dictionary<string, string> OptionsDict = new Dictionary<string, string>
        {
            { "TWILIO_ACCOUNT_SID", "ACXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" },
            { "TWILIO_API_KEY", "SKXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" },
            { "TWILIO_API_SECRET", "aXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" },
            { "SENDING_PHONE_NUMBER", "+15555555555" },
            { "APP_HASH", "xxxxxx" },
            { "CLIENT_SECRET", "ISXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" }
        };

        private readonly IOptions<AppSettings> _options;
        private AppSettings appSettings;
        private IOptions<MemoryCacheOptions> _cacheOptions;

        public SmsVerificationControllerTest()
        {
            // Arrange
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(OptionsDict)
                .Build();

            appSettings = new AppSettings();
            config.Bind(appSettings);
            _options = Options.Create(appSettings);
            _cacheOptions = Microsoft.Extensions.Options.Options.Create(new MemoryCacheOptions());
        }

        [Fact(Skip = "need mock library")]
        public void Test_request_verification_code()
        {
            // Arrange
            var cache = new MemoryCache(_cacheOptions);
            var controller = new SmsVerificationController(_options, cache);
            var testPhone = "+17075555555";

            var request = new GenericRequest()
            {
                client_secret = appSettings.CLIENT_SECRET,
                phone = testPhone
            };
            var result = (Dictionary<string, string>)controller.RequestVerificationCode(request).Value;

            Assert.True(result["success"] == "true");
            Assert.True(result["time"] == SmsVerificationHelper.VerificationCodeExp.ToString());
            Assert.False(string.IsNullOrEmpty((string)cache.Get(testPhone)));
        }
    }

}
