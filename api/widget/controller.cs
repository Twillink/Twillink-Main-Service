

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Twillink.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/widget")]
    public class WidgetController : ControllerBase
    {
        private readonly IWidgetService _IWidgetService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        public WidgetController(IWidgetService roleService)
        {
            _IWidgetService = roleService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
        }

        // [Authorize]
        [HttpGet]
        [Route("{Username}")]
        public async Task<object> Get([FromRoute] string Username)
        {
            try
            {
                var data = await _IWidgetService.Get(Username);
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