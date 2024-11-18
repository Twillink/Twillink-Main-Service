

using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Twillink.Server.Controllers
{
    [ApiController]
    [Route("api/v1/Attachment")]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _IAttachmentService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        private readonly IMongoCollection<MediaFile> AttachmentLink;
        private readonly ConvertJWT _ConvertJwt;
        public AttachmentController(IConfiguration configuration, IAttachmentService roleService, ConvertJWT convert)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Twillink");
            AttachmentLink = database.GetCollection<MediaFile>("MediaFile");
            _IAttachmentService = roleService;
            _errorUtility = new ErrorHandlingUtility();
            _masterValidationService = new ValidationMasterDto();
            _ConvertJwt = convert;
        }

        [Authorize]
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
        [HttpGet]
        [Route("Download/{fileId}")]
        public async Task<IActionResult> Download(string fileId)
        {
            try
            {
                // Retrieve the document from MongoDB
                var mediaFile = await AttachmentLink.Find(x => x.Id == fileId).FirstOrDefaultAsync();

                if (mediaFile == null)
                {
                    return NotFound(new { status = false, message = "File not found." });
                }

                // Set the Content-Disposition header to 'inline' to view in the browser
                Response.Headers.Add("Content-Disposition", $"inline; filename={mediaFile.FileName}");

                // Serve the file as a response
                return File(mediaFile.Data, mediaFile.ContentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = false, message = "An error occurred", details = ex.Message });
            }
        }



        [Authorize]
        [HttpPost]
        [Route("Upload")]
        public async Task<object> Upload(IFormFile file)
        {
            try
            {
                string accessToken = HttpContext.Request.Headers["Authorization"];
                string idUser = await _ConvertJwt.ConvertString(accessToken);

                if (file == null || file.Length == 0)
                {
                    throw new CustomException(400, "Message", "File not found");
                }

                // Define max file size: 200 MB (for any file type)
                const long maxFileSize = 200 * 1024 * 1024; // 200 MB

                // Validate file size
                if (file.Length > maxFileSize)
                {
                    throw new CustomException(400, "Message", "File size must not exceed 200 MB.");
                }

                // Convert file to byte array
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                // Create a MongoDB model
                var mediaDocument = new MediaFile
                {
                    Id = Guid.NewGuid().ToString(),
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    UploadedBy = idUser,
                    Data = fileBytes,
                    UploadedAt = DateTime.UtcNow
                };

                // Save the file to MongoDB
                await AttachmentLink.InsertOneAsync(mediaDocument);

                return Ok(new
                {
                    status = true,
                    message = "File uploaded successfully",
                    fileId = mediaDocument.Id,
                    path = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/v1/Attachment/Download/{mediaDocument.Id}"
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
                return StatusCode(500, new { status = false, message = "An error occurred", details = ex.Message });
            }
        }


        // [Authorize]
        // [HttpPost]
        // [Route("Upload")]
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

        // [HttpGet]
        // [Route("{fileName}")]
        // public IActionResult GetFile(string fileName)
        // {
        //     var filePath = Path.Combine("/home/hilyatulw/twillink/Twillink-Main-Service/publish/wwwroot/uploads", fileName);

        //     if (System.IO.File.Exists(filePath))
        //     {
        //         var fileBytes = System.IO.File.ReadAllBytes(filePath);
        //         return File(fileBytes, "application/octet-stream", fileName);
        //     }

        //     return NotFound();
        // }


        // [Authorize]
        // [HttpPost]
        // [Route("Upload")]
        // public async Task<IActionResult> UploadV2(IFormFile file)
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
        //         const long maxVideoSize = 200 * 1024 * 1024; // 50 MB

        //         // Check if the file is an image
        //         if (file.Length > maxImageSize)
        //         {
        //             throw new CustomException(400, "Message", "Media size must not exceed 200 MB.");
        //         }

        //         // Generate unique file name
        //         var fileName = $"{Guid.NewGuid()}-{file.FileName}";

        //         // Define the path to save the file
        //         var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //         if (!Directory.Exists(uploadPath))
        //         {
        //             Directory.CreateDirectory(uploadPath);
        //         }

        //         var filePath = Path.Combine(uploadPath, fileName);

        //         // Save the file locally
        //         using (var stream = new FileStream(filePath, FileMode.Create))
        //         {
        //             await file.CopyToAsync(stream);
        //         }

        //         // Return the file information
        //         return Ok(new
        //         {
        //             status = true,
        //             fileName,
        //             path = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/uploads/{fileName}" // Relative path for accessing the file
        //         });
        //     }
        //     catch (CustomException ex)
        //     {
        //         int errorCode = ex.ErrorCode;
        //         var errorResponse = new ErrorResponse(errorCode, ex.ErrorHeader, ex.Message);
        //         return _errorUtility.HandleError(errorCode, errorResponse);
        //     }
        //     catch (Exception ex)
        //     {
        //         // Handle unexpected errors
        //         return StatusCode(500, new
        //         {
        //             status = false,
        //             message = "An unexpected error occurred.",
        //             details = ex.Message
        //         });
        //     }
        // }

    }
}

// MongoDB model
public class MediaFile
{
    public string Id { get; set; } // Unique file ID
    public string FileName { get; set; } // Original file name
    public string ContentType { get; set; } // MIME type
    public long FileSize { get; set; } // File size in bytes
    public string UploadedBy { get; set; } // User ID who uploaded the file
    public byte[] Data { get; set; } // File data as byte array
    public DateTime UploadedAt { get; set; } // Upload timestamp
}
