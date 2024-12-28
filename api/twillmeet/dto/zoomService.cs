using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class ZoomService
{
    private readonly string _zoomApiKey = "fXnDPxR_RtgsMtDYhkRg"; // Replace with your API Key
    private readonly string _zoomApiSecret = "VRPRQ9gCEc5mpRbTA8JvGVPg8cSA0tav"; // Replace with your API Secret

    public async Task<object> CreateLinkMeet(string idUser)
    {
        try
        {
            // Generate JWT Token
            var token = "G6dDb8C4vSwHzHy9F3bQb-UQye1FeNwXQ";
            // Create Zoom Meeting Request
            var requestBody = new
            {
                topic = "Consultation Meeting",
                type = 2, // Scheduled meeting
                start_time = DateTime.UtcNow.AddHours(1).ToString("yyyy-MM-ddTHH:mm:ssZ"), // ISO 8601 format
                duration = 30, // 30 minutes
                settings = new
                {
                    join_before_host = true,
                    approval_type = 0 // No approval required
                }
            };

            using (var httpClient = new HttpClient())
            {

                httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {token}");
                var jsonBody = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"https://api.zoom.us/v2/users/me/meetings", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonConvert.DeserializeObject<object>(responseString);
                    return responseData;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Error creating meeting: {errorContent}");
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"An error occurred while creating the meeting: {ex.Message}", ex);
        }
    }

    private string GenerateJwtToken()
    {
        string key = "fXnDPxR_RtgsMtDYhkRg";
        string secret = "VRPRQ9gCEc5mpRbTA8JvGVPg8cSA0tav";

        string combined = $"{key}:{secret}";
        string base64Encoded = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(combined));
        return base64Encoded;
    }
}
