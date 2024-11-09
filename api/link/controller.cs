

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Twillink.Server.Controllers
{
    [ApiController]
    [Route("api/v1/link")]
    public class LinkUrlController : ControllerBase
    {
        private readonly ILinkUrlService _ILinkUrlService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        public LinkUrlController(ILinkUrlService LinkUrlService)
        {
            _ILinkUrlService = LinkUrlService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
        }

        [HttpGet("check/{userName}")]
        public async Task<object> GetById([FromRoute] string userName)
        {
            try
            {
                var data = await _ILinkUrlService.GetById(userName);
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