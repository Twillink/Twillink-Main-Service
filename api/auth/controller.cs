
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Twillink.Server.Controllers
{
    [ApiController]
    [Route("api/v1/user-auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _IAuthService;
        private readonly IEmailService _emailService;

        private readonly ConvertJWT _ConvertJwt;
        private readonly ErrorHandlingUtility _errorUtility;

        private readonly ValidationAuthDto _masterValidationService;
        private readonly string key1;
        private readonly string key2;

        public AuthController(IConfiguration configuration, IEmailService EmailService, IAuthService authService, ConvertJWT convert, ValidationAuthDto MasterValidationService)
        {
            _IAuthService = authService;
            _emailService = EmailService;
            _ConvertJwt = convert;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = MasterValidationService;
            this.key1 = configuration.GetSection("Zoom")["SdkKey"];
            this.key2 = configuration.GetSection("Zoom")["SdkSecret"];

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto login)
        {
            try
            {
                var validationErrors = _masterValidationService.ValidateRegister(login);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
                var dataList = await _IAuthService.RegisterAsync(login);
                return Ok(dataList);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<object> LoginAsync([FromBody] LoginDto login)
        {
            try
            {
                var validationErrors = _masterValidationService.ValidateLogin(login);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
                var response = await _IAuthService.LoginAsync(login);
                return Ok(response);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("loginGoogle")]
        public async Task<object> LoginGoogleAsync([FromBody] LoginGoogleDto login)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(login.Token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var payloadJson = jsonToken.Payload.SerializeToJson();
                    var payload = JsonSerializer.Deserialize<JwtPayloads>(payloadJson);
                    if (payload.Audience == "twillink-de")
                    {
                        var response = await _IAuthService.LoginGoogleAsync(payload.Email);
                        return Ok(response);
                    }
                    else
                    {
                        throw new CustomException(400, "Token", "Invalid Token");
                    }
                }
                else
                {
                    throw new CustomException(400, "Token", "Invalid Token");
                }

            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("registerGoogle")]
        public async Task<object> RegisterGoogleAsync([FromBody] RegisterGoogleDto login)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(login.Token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var payloadJson = jsonToken.Payload.SerializeToJson();
                    var payload = JsonSerializer.Deserialize<JwtPayloads>(payloadJson);
                    if (payload.Audience == "twillink-de")
                    {
                        var response = await _IAuthService.RegisterGoogleAsync(payloadJson, login);
                        return Ok(response);
                    }
                    else
                    {
                        throw new CustomException(400, "Token", "Invalid Token");
                    }
                }
                else
                {
                    throw new CustomException(400, "Token", "Invalid Token");
                }

            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<object> ForgotPassword([FromBody] UpdateUserAuthDto item)
        {
            try
            {
                var dataList = await _IAuthService.ForgotPasswordAsync(item);
                return Ok(dataList);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("change-password")]
        public async Task<object> UpdatePassword([FromBody] ChangeUserPasswordDto item)
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var dataList = await _IAuthService.UpdatePassword(idUser, item);
                return Ok(dataList);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpGet]
        [Route("check-mail-registered/{email}")]
        public async Task<object> CheckMail([FromRoute] string email)
        {
            try
            {
                var dataList = await _IAuthService.CheckMail(email);
                return Ok(dataList);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        // [AllowAnonymous]
        // [HttpGet]
        // [Route("activation/{UID}")]
        // public async Task<object> VerifySeasonsAsync([FromRoute] string UID)
        // {
        //     try
        //     {
        //         var dataList = await _IAuthService.Aktifasi(UID);
        //         return Ok(dataList);
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }

        [Authorize]
        [HttpGet]
        [Route("verifySessions")]
        public object Aktifasi()
        {
            try
            {
                var claims = User.Claims;
                if (claims == null)
                {
                    return new CustomException(400, "Error", "Unauthorized");
                }
                return new { code = 200, message = "not expired" };
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("zoomget")]
        public async Task<object> Zoomget([FromBody] ZoomGetDto request)
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                // Validate request
                if (string.IsNullOrEmpty(request.MeetingNumber) || request.Role < 0 || request.Role > 1)
                {
                    return BadRequest(new { Error = "Invalid request. Ensure 'MeetingNumber' and 'Role' are valid." });
                }

                // Get Zoom SDK credentials from environment variables or appsettings
                var sdkKey = key1 ?? throw new Exception("SDK Key missing");
                var sdkSecret = key2 ?? throw new Exception("SDK Secret missing");

                // Generate the JWT token
                var iat = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var exp = iat + (request.ExpirationSeconds ?? 7200); // Default to 2 hours if not specified

                var payload = new JwtPayload
            {
                { "appKey", sdkKey },
                { "sdkKey", sdkKey },
                { "mn", request.MeetingNumber },
                { "role", request.Role },
                { "iat", iat },
                { "exp", exp },
                { "tokenExp", exp }
            };

                var header = new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(sdkSecret)),
                    SecurityAlgorithms.HmacSha256));

                var token = new JwtSecurityToken(header, payload);
                var signature = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { Signature = signature });
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }
    }

}