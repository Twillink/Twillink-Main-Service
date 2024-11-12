using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Twillink.Server.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class CountryController : ControllerBase
    {
        private readonly ILinkUrlService _ILinkUrlService;
        private readonly ErrorHandlingUtility _errorUtility;
        private readonly ValidationMasterDto _masterValidationService;
        public CountryController(ILinkUrlService LinkUrlService)
        {
            _ILinkUrlService = LinkUrlService;
            _errorUtility = new ErrorHandlingUtility();
        }

        [HttpGet("country")]
        public async Task<object> GetById()
        {
            try
            {
                // Define the path to your JSON file
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "api/country", "country.json");

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("JSON file not found.");
                }

                // Read the JSON file content asynchronously
                var jsonData = await System.IO.File.ReadAllTextAsync(filePath);

                // Optionally, deserialize the JSON data if you want to work with it as an object
                // var countries = JsonSerializer.Deserialize<List<Country>>(jsonData);

                // Return the JSON data as a response
                return Ok(JsonDocument.Parse(jsonData).RootElement);
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