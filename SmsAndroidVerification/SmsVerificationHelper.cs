using System;
using System.IO.MemoryMappedFiles;
using Microsoft.Extensions.Caching.Memory;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SmsAndroidVerification
{
    public class SmsVerificationHelper
    {
        private readonly string _appHash;
        public static int VerificationCodeExp = 900;
        private readonly IMemoryCache _cache;
        private readonly PhoneNumber _sendingPhoneNumber;

        public SmsVerificationHelper(IMemoryCache cache, AppSettings settings)
        {
            _cache = cache;
            _sendingPhoneNumber = new PhoneNumber(settings.SENDING_PHONE_NUMBER);
            _appHash = settings.APP_HASH;
            

            // Initialize Twilio client. Only needs to be done once as the library stores it internally.
            TwilioClient.Init(settings.TWILIO_API_KEY, settings.TWILIO_API_SECRET, settings.TWILIO_ACCOUNT_SID);
        }

        public string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        static DateTime GetExpirationDateTimeOffset()
        {
            var currentTime = DateTime.Now;
            return currentTime.AddSeconds(VerificationCodeExp);
        }

        public string GenerateMessageWithVerificationCode(string phoneNumberStr)
        {
            var verificationCode = GenerateVerificationCode();
            // save verification code, phone number pair to cache
            var _cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(GetExpirationDateTimeOffset());

            _cache.Set(phoneNumberStr, verificationCode, _cacheOptions);
            return $"[#] Use {verificationCode} as your code for the app!\n{_appHash}";
        }

        public void SendVerificationCode(string phoneNumberStr)
        {
            var message = GenerateMessageWithVerificationCode(phoneNumberStr);
            var phoneNumber = new PhoneNumber(phoneNumberStr);

            MessageResource.Create(phoneNumber, @from: _sendingPhoneNumber, body: message);
        }

        public bool VerifyCode(string phoneNumberStr, string smsMessage)
        {
            var verificationCode = (string)_cache.Get(phoneNumberStr);

            return smsMessage.IndexOf(verificationCode, StringComparison.Ordinal) > -1;
        }

        public void ResetVerificationCodeForNumber(string phoneNumberStr)
        {
             var verificationCode = (string)_cache.Get(phoneNumberStr);

            if (!string.IsNullOrEmpty(verificationCode))
            {
                _cache.Remove(phoneNumberStr);
            }
        }
    }
}