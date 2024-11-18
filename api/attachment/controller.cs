

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
        private readonly ConvertJWT _ConvertJwt;
        public AttachmentController(IAttachmentService roleService, ConvertJWT convert)
        {
            _IAttachmentService = roleService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
            _ConvertJwt = convert;
        }

        [HttpDelete]
        [Route("{UrlLink}")]
        public async Task<object> GetAll([FromRoute] string UrlLink)
        {
            try
            {
                var data = await _IAttachmentService.DeleteFileAsync(UrlLink);
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
        [HttpGet]
        [Route("GetAll")]
        public async Task<object> Get()
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);

                var data = await _IAttachmentService.Get(idUser);
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
        // [HttpPost]
        // [Route("UploadV2")]
        // public async Task<object> Upload(IFormFile file)
        // {

        //     try
        //     {
        //         string accessToken = HttpContext.Request.Headers["Authorization"];
        //         string idUser = await _ConvertJwt.ConvertString(accessToken);

        //         if (file == null || file.Length == 0)
        //         {
        //             throw new CustomException(400, "Message", "File Not Found");
        //         }

        //         // Define allowed MIME types for images and videos
        //         var allowedImageTypes = new[] { "application/pdf", "image/jpeg", "image/png", "image/gif", "image/jpeg" };
        //         var allowedVideoTypes = new[] { "video/mp4", "video/mpeg", "video/quicktime" };

        //         // Define max file size: 2 MB for images, 50 MB for videos
        //         const long maxImageSize = 200 * 1024 * 1024; // 2 MB
        //         const long maxVideoSize = 300 * 1024 * 1024; // 50 MB

        //         // Check if the file is an image
        //         if (Array.Exists(allowedImageTypes, type => type.Equals(file.ContentType, StringComparison.OrdinalIgnoreCase)))
        //         {
        //             if (file.Length > maxImageSize)
        //             {
        //                 throw new CustomException(400, "Message", "Image size must not exceed 2 MB.");
        //             }
        //         }
        //         // Check if the file is a video
        //         else if (Array.Exists(allowedVideoTypes, type => type.Equals(file.ContentType, StringComparison.OrdinalIgnoreCase)))
        //         {
        //             if (file.Length > maxVideoSize)
        //             {
        //                 throw new CustomException(400, "Message", "Video size must not exceed 50 MB.");
        //             }
        //         }
        //         else
        //         {
        //             throw new CustomException(400, "Message", "Only image (JPEG, PNG, GIF) or video (MP4, MPEG, MOV) files are allowed.");
        //         }
        //         // Generate unique file name
        //         var fileName = $"{Guid.NewGuid()}-{file.FileName}";
        //         // Upload file
        //         var result = await _IAttachmentService.Upload(file, fileName, idUser);
        //         return Ok(new
        //         {
        //             status = true,
        //             fileName = result.FileName,
        //             path = result.Url
        //         });
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        // }

        [HttpGet]
        [Route("{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            var filePath = Path.Combine("/home/hilyatulw/twillink/Twillink-Main-Service/publish/wwwroot/uploads", fileName);

            if (System.IO.File.Exists(filePath))
            {
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/octet-stream", fileName);
            }

            return NotFound();
        }


        [Authorize]
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> UploadV2(IFormFile file)
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);

                if (file == null || file.Length == 0)
                {
                    throw new CustomException(400, "Message", "File Not Found");
                }

                // Define allowed MIME types for images and videos
                var allowedImageTypes = new[] { "application/pdf", "image/jpeg", "image/png", "image/gif", "image/jpeg" };
                var allowedVideoTypes = new[] { "video/mp4", "video/mpeg", "video/quicktime" };

                // Define max file size: 2 MB for images, 50 MB for videos
                const long maxImageSize = 200 * 1024 * 1024; // 2 MB
                const long maxVideoSize = 200 * 1024 * 1024; // 50 MB

                // Check if the file is an image
                if (file.Length > maxImageSize)
                {
                    throw new CustomException(400, "Message", "Media size must not exceed 200 MB.");
                }

                // Generate unique file name
                var fileName = $"{Guid.NewGuid()}-{file.FileName}";

                // Define the path to save the file
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var filePath = Path.Combine(uploadPath, fileName);

                // Save the file locally
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the file information
                return Ok(new
                {
                    status = true,
                    fileName,
                    path = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/uploads/{fileName}" // Relative path for accessing the file
                });
            }
            catch (CustomException ex)
            {
                int errorCode = ex.ErrorCode;
                var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
                return _errorUtility.HandleError(errorCode, errorResponse);
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                return StatusCode(500, new
                {
                    status = false,
                    message = "An unexpected error occurred.",
                    details = ex.Message
                });
            }
        }

    }
}