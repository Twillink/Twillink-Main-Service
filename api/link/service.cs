using MongoDB.Driver;
using Twillink.Shared.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Calendar.v3.Data;


namespace RepositoryPattern.Services.LinkUrlService
{
    public class LinkUrlService : ILinkUrlService
    {
        private readonly IMongoCollection<User> dataUser;
        private readonly string key;

        public LinkUrlService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Twillink");
            dataUser = database.GetCollection<User>("Users");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];

        }

        public async Task<Object> GetById(string UserName)
        {
            try
            {
                var items = await dataUser.Find(_ => _.Username == UserName).FirstOrDefaultAsync();
                if (items == null)
                {
                    return new { code = 200, message = "Available" };
                }
                else
                {
                    throw new CustomException(400, "Message", "Not Available");
                }
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<Object> gmeetcreate(string UserName)
        {
            try
            {
                // var eventItem = new Event
                // {
                //     Summary = "Important Meeting",
                //     Start = new EventDateTime { DateTime = DateTime.Now.AddDays(1).AddHours(3) },
                //     End = new EventDateTime { DateTime = DateTime.Now.AddDays(1).AddHours(4) },
                //     Visibility = "public",
                //     ConferenceData = new ConferenceData
                //     {
                //         CreateRequest = new CreateConferenceRequest
                //         {
                //             RequestId = Guid.NewGuid().ToString()
                //         }
                //     }
                // };

                // var credentialPath = Path.Combine(AppContext.BaseDirectory, "credent.json");
                // var credential = GoogleCredential.FromFile(credentialPath) s
                //     .CreateScoped(CalendarService.Scope.Calendar);
                // var service = new CalendarService(new BaseClientService.Initializer()
                // {
                //     HttpClientInitializer = credential,
                //     ApplicationName = "Twillink",
                // });

                // var request = service.Events.Insert(eventItem, "primary");
                // request.ConferenceDataVersion = 1;
                var createdEvent = "request.Execute()";
                return new { code = 200, link = createdEvent };
            }
            catch (Google.GoogleApiException ex)
            {
                throw new CustomException(400, "Message", ex.Message);
            }
        }
    }
}