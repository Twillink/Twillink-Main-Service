using MongoDB.Driver;
using Twillink.Shared.Models;

namespace RepositoryPattern.Services.LinkUrlService
{
    public class LinkUrlService : ILinkUrlService
    {
        private readonly IMongoCollection<Link> dataUser;
        private readonly string key;

        public LinkUrlService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Twillink");
            dataUser = database.GetCollection<Link>("Link");
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }

        public async Task<Object> GetById(string UserName)
        {
            try
            {
                var items = await dataUser.Find(_ => _.UserName == UserName).FirstOrDefaultAsync();
                return new { code = 200, data = items, message = "Available" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
    }
}