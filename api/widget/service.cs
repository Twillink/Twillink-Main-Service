using MongoDB.Driver;
using Twillink.Shared.Models;

namespace RepositoryPattern.Services.WidgetService
{
    public class WidgetService : IWidgetService
    {
        private readonly IMongoCollection<Widget> widgetLink;
        private readonly IMongoCollection<AddText> addText;
        private readonly IMongoCollection<AddLink> addLink;


        private readonly IMongoCollection<User> users;

        private readonly string key;

        public WidgetService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Twillink");
            widgetLink = database.GetCollection<Widget>("Widget");
            addText = database.GetCollection<AddText>("Widget");
            addLink = database.GetCollection<AddLink>("Widget");

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

                var WidgetList = await addText.Find(_ => _.UserId == items.Id).SortBy(_ => _.sequence).ToListAsync();
                var widget = new
                {
                    WidgetList,
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

        public async Task<Object> GetUser(string Id)
        {
            try
            {
                var items = await users.Find(_ => _.Id == Id).FirstOrDefaultAsync();
                if (items == null)
                {
                    throw new CustomException(404, "Message", "Account Not Found");
                }

                var WidgetList = await addText.Find(_ => _.UserId == Id).SortBy(_ => _.sequence).ToListAsync();
                var widget = new
                {
                    WidgetList,
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

        public async Task<object> AddText(string idUser, CreateText createText)
        {
            try
            {
                var items = await addText.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                var uuid = Guid.NewGuid().ToString();
                var itemNew = new AddText()
                {
                    Id = uuid,
                    UserId = idUser,
                    sequence = count + 1,
                    typeWidget = "text",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Text = createText.Text,
                    }
                };

                await addText.InsertOneAsync(itemNew);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> AddLink(string idUser, CreateLink createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                var uuid = Guid.NewGuid().ToString();
                var itemNew = new AddLink()
                {
                    Id = uuid,
                    UserId = idUser,
                    sequence = count + 1,
                    typeWidget = "link",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Title = createText.Title,
                        Url = createText.Url,
                    }
                };

                await addLink.InsertOneAsync(itemNew);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> AddImage(string idUser, CreateImage createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                var uuid = Guid.NewGuid().ToString();
                var itemNew = new AddLink()
                {
                    Id = uuid,
                    UserId = idUser,
                    sequence = count + 1,
                    typeWidget = "image",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Caption = createText.Caption,
                        Url = createText.Url,
                    }
                };

                await addLink.InsertOneAsync(itemNew);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> AddVideo(string idUser, CreateImage createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                var uuid = Guid.NewGuid().ToString();
                var itemNew = new AddLink()
                {
                    Id = uuid,
                    UserId = idUser,
                    sequence = count + 1,
                    typeWidget = "video",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Caption = createText.Caption,
                        Url = createText.Url,
                    }
                };

                await addLink.InsertOneAsync(itemNew);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> AddBlog(string idUser, CreateContent createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                var uuid = Guid.NewGuid().ToString();
                var itemNew = new AddLink()
                {
                    Id = uuid,
                    UserId = idUser,
                    sequence = count + 1,
                    typeWidget = "blog",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Title = createText.Title,
                        Url = createText.Url,
                        Contents = createText.Content,
                    }
                };

                await addLink.InsertOneAsync(itemNew);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> AddMap(string idUser, CreateMap createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                foreach (var item in items)
                {
                    if (item.typeWidget == "map")
                    {
                        // Define the filter by Id
                        var filter = Builders<AddLink>.Filter.Eq(_ => _.Id, item.Id);

                        // Define the update for the Content field
                        var update = Builders<AddLink>.Update.Set(_ => _.Content, new Content
                        {
                            Caption = createText.Caption,
                            Latitude = createText.Latitude,
                            Longitude = createText.Longitude,
                        });

                        // Perform the update
                        await addLink.UpdateOneAsync(filter, update);
                        return new { code = 200, message = "Berhasil" };
                    }
                }

                var uuid = Guid.NewGuid().ToString();
                var itemNew = new AddLink()
                {
                    Id = uuid,
                    UserId = idUser,
                    sequence = count + 1,
                    typeWidget = "map",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Caption = createText.Caption,
                        Latitude = createText.Latitude,
                        Longitude = createText.Longitude,
                    }
                };

                await addLink.InsertOneAsync(itemNew);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> AddContact(string idUser, CreateContact createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                foreach (var item in items)
                {
                    if (item.typeWidget == "contact")
                    {
                        // Define the filter by Id
                        var filter = Builders<AddLink>.Filter.Eq(_ => _.Id, item.Id);

                        // Define the update for the Content field
                        var update = Builders<AddLink>.Update.Set(_ => _.Content, new Content
                        {
                            Email = createText.Email,
                            PhoneNumber = createText.PhoneNumber
                        });

                        // Perform the update
                        await addLink.UpdateOneAsync(filter, update);
                        return new { code = 200, message = "Berhasil" };
                    }
                }

                var uuid = Guid.NewGuid().ToString();
                var itemNew = new AddLink()
                {
                    Id = uuid,
                    UserId = idUser,
                    sequence = null,
                    typeWidget = "contact",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Email = createText.Email,
                        PhoneNumber = createText.PhoneNumber,
                    }
                };

                await addLink.InsertOneAsync(itemNew);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> AddCarousel(string idUser, CreateCarausel createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                foreach (var item in items)
                {
                    if (item.typeWidget == "carousel")
                    {
                        // Define the filter by Id
                        var filter = Builders<AddLink>.Filter.Eq(_ => _.Id, item.Id);

                        // Define the update for the Content field
                        var update = Builders<AddLink>.Update.Set(_ => _.Content, new Content
                        {
                            Caption = createText.Caption,
                            AttachmentUrl = createText.AttachmentUrl
                        });

                        // Perform the update
                        await addLink.UpdateOneAsync(filter, update);
                        return new { code = 200, message = "Berhasil" };
                    }
                }

                var uuid = Guid.NewGuid().ToString();
                var itemNew = new AddLink()
                {
                    Id = uuid,
                    UserId = idUser,
                    sequence = count + 1,
                    typeWidget = "carousel",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Caption = createText.Caption,
                        AttachmentUrl = createText.AttachmentUrl
                    }
                };

                await addLink.InsertOneAsync(itemNew);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

    }
}