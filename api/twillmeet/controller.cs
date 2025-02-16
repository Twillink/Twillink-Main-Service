

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Twillink.Server.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("/api/v1/[controller]")]
    public class TwilmeetController : ControllerBase
    {
        private readonly ITwilmeetService _ITwilmeetService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationTwilmeetDto _masterValidationService;
        private readonly ConvertJWT _ConvertJwt;
        public TwilmeetController(ITwilmeetService TwilmeetService, ConvertJWT convert)
        {
            _ITwilmeetService = TwilmeetService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationTwilmeetDto();
            _ConvertJwt = convert;
        }

        // [Authorize]
        [HttpGet]
        public async Task<object> Get()
        {
            try
            {
                var data = await _ITwilmeetService.Get();
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<object> GetById([FromRoute] string id)
        {
            try
            {
                var data = await _ITwilmeetService.GetById(id);
                return Ok(data);
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
        public async Task<object> Post([FromBody] CreateTwilmeetDto item)
        {
            try
            {
                var validationErrors = _masterValidationService.ValidateCreateInput(item);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ITwilmeetService.Post(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [HttpPost("BuyTweelmeet")]
        public async Task<object> PostBuy([FromBody] CreateBuyTwilmeetDto item)
        {
            try
            {
                var validationErrors = _masterValidationService.ValidateCreateBuyInput(item);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);
                var data = await _ITwilmeetService.PostBuy(item, idUser);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpPost("BuyTweelmeet/{id}")]
        public async Task<object> PostApprovalBuy([FromRoute] string id)
        {
            try
            {
                var data = await _ITwilmeetService.PostApproval(id);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpGet("BuyTweelmeet/{id}")]
        public async Task<object> GetMember([FromRoute] string id)
        {
            try
            {
                var data = await _ITwilmeetService.GetMemberWebinar(id);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        // [Authorize]
        [HttpPut("{id}")]
        public async Task<object> Put([FromRoute] string id, [FromBody] CreateTwilmeetDto item)
        {
            try
            {
                var validationErrors = _masterValidationService.ValidateCreateInput(item);
                if (validationErrors.Count > 0)
                {
                    var errorResponse = new { code = 400, errorMessage = validationErrors };
                    return BadRequest(errorResponse);
                }
                var data = await _ITwilmeetService.Put(id, item);
                return Ok(data);
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<object> Delete([FromRoute] string id)
        {
            try
            {
                var data = await _ITwilmeetService.Delete(id);
                return Ok(data);
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