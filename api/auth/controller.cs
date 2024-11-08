
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Twillink.Server.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _IAuthService;
        private readonly IEmailService _emailService;

        private readonly ConvertJWT _ConvertJwt;
        private readonly ErrorHandlingUtility _errorUtility;

        private readonly ValidationAuthDto _masterValidationService;
        public AuthController(IEmailService EmailService, IAuthService authService, ConvertJWT convert, ValidationAuthDto MasterValidationService)
        {
            _IAuthService = authService;
            _emailService = EmailService;
            _ConvertJwt = convert;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = MasterValidationService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("auth/login")]
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
        [Route("auth/register")]
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

        [Authorize]
        [HttpPost]
        [Route("auth/updatePassword")]
        public async Task<object> UpdatePassword([FromBody] UpdatePasswordDto item)
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var validationErrors = _masterValidationService.ValidateUpdatePassword(item);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
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

        [AllowAnonymous]
        [HttpPost]
        [Route("auth/verifyOtp")]
        public async Task<object> VerifyOtp([FromBody] OtpDto otp)
        {
            try
            {
                var dataList = await _IAuthService.VerifyOtp(otp);
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
        [Route("auth/requestOtpEmail")]
        public async Task<object> RequestOtp([FromBody] ReqOtpDto otp)
        {
            try
            {
                var dataList = await _IAuthService.RequestOtpEmail(otp.Email);
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
        [HttpGet]
        [Route("auth/activation/{UID}")]
        public async Task<object> VerifySeasonsAsync([FromRoute] string UID)
        {
            try
            {
                var dataList = await _IAuthService.Aktifasi(UID);
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
        [HttpGet]
        [Route("auth/verifySessions")]
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
    }

}