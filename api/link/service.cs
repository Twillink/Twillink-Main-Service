using MongoDB.Driver;
using Twillink.Shared.Models;

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
                if(items == null)
                {
                    return new { code = 200, message = "Available" };
                }else{
                    throw new CustomException(400, "Message", "Not Available");
                }
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}