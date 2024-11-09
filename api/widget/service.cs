using MongoDB.Driver;
using Twillink.Shared.Models;

namespace RepositoryPattern.Services.WidgetService
{
    public class WidgetService : IWidgetService
    {
        private readonly IMongoCollection<Widget> widgetLink;
        private readonly IMongoCollection<User> users;

        private readonly string key;

        public WidgetService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Twillink");
            widgetLink = database.GetCollection<Widget>("Widget");
            users = database.GetCollection<User>("Users");

            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }
        public async Task<Object> Get(string Username)
        {
            try
            {
                var items = await users.Find(_ => _.Username.ToLower() == Username.ToLower()).FirstOrDefaultAsync();
                if (items == null)
                {
                    throw new CustomException(404, "Message", "Account Not Found");
                }
                var widget = new Widget
                {
                    WidgetList = new object[]
                    {
                        // new { Id = 1, Name = "Widget 1" },
                        // new { Id = 2, Name = "Widget 2" }
                    },
                    user = new
                    {
                        Email = items.Email,
                        FullName = items.FullName,
                        PhoneNumber = items.PhoneNumber,
                        Username = items.Username
                    }
                };
                return new { code = 200, data = widget, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}