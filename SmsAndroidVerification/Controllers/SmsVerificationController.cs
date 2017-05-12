using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace SmsAndroidVerification.Controllers
{
    [Produces("application/json")]
    public class SmsVerificationController : Controller
    {
        private AppSettings _appSettings;
        private SmsVerificationHelper _smsVerificationHelper;

        public SmsVerificationController(IOptions<AppSettings> appSettings, IMemoryCache cache)
        {
            _appSettings = appSettings.Value;
            _smsVerificationHelper = new SmsVerificationHelper(cache, _appSettings);
        }

        [HttpPost("/api/request")]
        public ObjectResult RequestVerificationCode([FromBody] GenericRequest request)
        {
            string clientSecret = request.clientSecret;
            string phone = request.phone;

            if (_appSettings.CLIENT_SECRET != clientSecret)
            {
                return StatusCode(500, "Client secret does not match.");
            }

            _smsVerificationHelper.SendVerificationCode(phone);

            return new ObjectResult(new Dictionary<string, string>()
            {
                {"success", "true" },
                {"time", SmsVerificationHelper.VerificationCodeExp.ToString() }
            });
        }

        [HttpPost("/api/verify")]
        public ObjectResult Verify([FromBody] VerificationRequest request)
        {
            string clientSecret = request.client_secret;
            string phone = request.phone;
            string smsMessage = request.sms_message;

            if (clientSecret == null & phone == null & smsMessage == null)
            {
                return StatusCode(500, "The client_secret, phone and sms_message parameters are required.");
            }

            if (_appSettings.CLIENT_SECRET != clientSecret)
            {
                return StatusCode(500, "The client_secret parameter does not match.");
            }

            var isVerified = _smsVerificationHelper.VerifyCode(phone, smsMessage);

            var responseDict =  new Dictionary<string, string>()
            {
                {"success", isVerified.ToString() }
            };

            if (isVerified)
            {
                responseDict.Add("phone", phone);
            }
            else
            {
                responseDict.Add("msg", "Unable to validate code for this phone number.");
            }

            return new ObjectResult(responseDict);
        }
        
        [HttpPost("/api/reset")]
        public ObjectResult ResetVerificationCode([FromBody] ResetRequest request)
        {
            string clientSecret = request.client_secret;
            string phone = request.phone;

            if (clientSecret == null | phone == null)
            {
                return StatusCode(500, "The client_secret and phone parameters are required.");
            }

            if (_appSettings.CLIENT_SECRET != clientSecret)
            {
                return StatusCode(500, "The client_secret parameter does not match.");
            }
            
            _smsVerificationHelper.ResetVerificationCodeForNumber(phone);

            return new ObjectResult(new Dictionary<string, string>()
            {
                {"success", "true" },
                {"phone", phone }
            });
        }

        [HttpGet("/config")]
        public JsonResult Config()
        {
            return new JsonResult(_appSettings);
        }
        
    }

    public class ResetRequest
    {
        public string client_secret { get; set; }
        public string phone { get; set; }
    }

    public class VerificationRequest
    {
        public string client_secret { get; set; }
        public string phone { get; set; }
        public string sms_message { get; set; }
    }

    public class GenericRequest
    {
        public string clientSecret { get; set; }
        public string phone { get; set; }
    }
}
