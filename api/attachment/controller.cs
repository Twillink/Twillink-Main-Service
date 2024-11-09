

using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Twillink.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/Attachment")]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _IAttachmentService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        public AttachmentController(IAttachmentService roleService)
        {
            _IAttachmentService = roleService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
            
        }

        // [Authorize]
        [HttpGet]
        [Route("{id}")]
        public async Task<object> Get([FromRoute] string id)
        {
            try
            {
                var data = await _IAttachmentService.Get(id);
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
        [Route("Upload")]
        public async Task<object> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File not selected.");
            }

            try
            {
                // Generate unique file name
                var fileName = $"{Guid.NewGuid()}-{file.FileName}";
                // Upload file
                var result = await _IAttachmentService.Upload(file, fileName);
                return Ok(new
                {
                    status = true,
                    fileName = result.FileName,
                    path = result.Url
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = false, message = ex.Message });
            }
        }
    }
}