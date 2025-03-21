using System.Text.RegularExpressions;
using MongoDB.Driver;
using Twillink.Shared.Models;

namespace RepositoryPattern.Services.WidgetService
{
    public class WidgetService : IWidgetService
    {
        private readonly IMongoCollection<Widget> widgetLink;
        private readonly IMongoCollection<AddText> addText;
        private readonly IMongoCollection<AddLink> addLink;
        private readonly IMongoCollection<AddSosmed> addSosmed;
        private readonly IMongoCollection<User> users;

        private readonly string key;

        public WidgetService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Twillink");
            widgetLink = database.GetCollection<Widget>("Widget");
            addText = database.GetCollection<AddText>("Widget");
            addLink = database.GetCollection<AddLink>("Widget");
            addSosmed = database.GetCollection<AddSosmed>("Sosmed");
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

                var filteredWidget = await addText.Find(_ => _.UserId == items.Id).SortBy(_ => _.sequence).ToListAsync();
                var WidgetList = filteredWidget.Where(x => x.typeWidget != "contact" || x.typeWidget != "profile" || x.typeWidget != "sosmed").Select(x => new
                {
                    x.Id,
                    WidgetText = x.typeWidget == "text" ? x.Content : null, // Only set if typeWidget is "text"
                    WidgetVideo = x.typeWidget == "video" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetLink = x.typeWidget == "link" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetCarousel = x.typeWidget == "carousel" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetBlog = x.typeWidget == "blog" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetMap = x.typeWidget == "map" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetContact = x.typeWidget == "contact" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetImage = x.typeWidget == "image" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetSchedule = x.typeWidget == "schedule" ? x.Content : null, // Only set if typeWidget is "schedule"
                    WidgetWebinar = x.typeWidget == "webinar" ? x.Content : null, // Only set if typeWidget is "webinar"
                    WidgetPdf = x.typeWidget == "pdf" ? x.Content : null, // Only set if typeWidget is "pdf"


                    x.CreatedAt,
                    x.UpdatedAt,
                    x.UserId,
                    x.sequence,
                    x.typeWidget,
                    x.width
                }).ToList();
                var WidgetContact = filteredWidget.Find(x => x.typeWidget == "contact");
                var WidgetProfile = filteredWidget.Find(x => x.typeWidget == "profile");
                var WidgetSosmed = await addSosmed.Find(_ => _.UserId.ToLower() == items.Id.ToLower()).ToListAsync();
                string pattern = @"[^/]+$";
                var filteredSosmed = WidgetSosmed.Select(x => new
                {
                    Key = Regex.Match(x.Key, pattern).Value,
                    Value = x.Value
                }).ToList();

                var widget = new
                {
                    WidgetList,
                    Profile = new
                    {
                        Username = items.Username,
                        Email = WidgetContact?.Content?.Email,
                        PhoneNumber = WidgetContact?.Content?.PhoneNumber,
                        FullName = WidgetProfile?.Content?.FullName,
                        Description = WidgetProfile?.Content?.Description,
                        UrlBanner = WidgetProfile?.Content?.UrlBanner,
                        UrlImage = WidgetProfile?.Content?.UrlImageProfile,
                    },
                    Sosmed = filteredSosmed

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

                var filteredWidget = await addText.Find(_ => _.UserId == items.Id).SortBy(_ => _.sequence).ToListAsync();
                var WidgetList = filteredWidget.Where(x => x.typeWidget != "contact" || x.typeWidget != "profile" || x.typeWidget != "sosmed").Select(x => new
                {
                    x.Id,
                    WidgetText = x.typeWidget == "text" ? x.Content : null, // Only set if typeWidget is "text"
                    WidgetVideo = x.typeWidget == "video" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetLink = x.typeWidget == "link" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetCarousel = x.typeWidget == "carousel" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetBlog = x.typeWidget == "blog" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetMap = x.typeWidget == "map" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetContact = x.typeWidget == "contact" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetImage = x.typeWidget == "image" ? x.Content : null, // Only set if typeWidget is "video"
                    WidgetSchedule = x.typeWidget == "schedule" ? x.Content : null, // Only set if typeWidget is "schedule"
                    WidgetWebinar = x.typeWidget == "webinar" ? x.Content : null, // Only set if typeWidget is "webinar"
                    WidgetPdf = x.typeWidget == "pdf" ? x.Content : null, // Only set if typeWidget is "pdf"

                    x.CreatedAt,
                    x.UpdatedAt,
                    x.UserId,
                    x.sequence,
                    x.typeWidget,
                    x.width
                }).ToList();
                var WidgetContact = filteredWidget.Find(x => x.typeWidget == "contact");
                var WidgetProfile = filteredWidget.Find(x => x.typeWidget == "profile");
                var WidgetSosmed = await addSosmed.Find(_ => _.UserId.ToLower() == items.Id.ToLower()).ToListAsync();
                string pattern = @"[^/]+$";
                var filteredSosmed = WidgetSosmed.Select(x => new
                {
                    Key = Regex.Match(x.Key, pattern).Value,
                    Value = x.Value
                }).ToList();

                var widget = new
                {
                    WidgetList,
                    Profile = new
                    {
                        Username = items.Username,
                        Email = WidgetContact?.Content?.Email,
                        PhoneNumber = WidgetContact?.Content?.PhoneNumber,
                        FullName = WidgetProfile?.Content?.FullName,
                        Description = WidgetProfile?.Content?.Description,
                        UrlBanner = WidgetProfile?.Content?.UrlBanner,
                        UrlImage = WidgetProfile?.Content?.UrlImageProfile,
                    },
                    Sosmed = filteredSosmed

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
                        urlThumbnail = createText.UrlThumbnail
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

        public async Task<object> AddPdf(string idUser, CreateImage createText)
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
                    typeWidget = "pdf",
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
                        urlThumbnail = createText.UrlThumbnail
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

        public async Task<object> AddWebinar(string idUser, CreateWebinar createText)
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
                    typeWidget = "webinar",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Title = createText.Title,
                        UrlWebinar = createText.UrlWebinar,
                        urlThumbnail = createText.UrlThumbnail,
                        Description = createText.Description,
                        Notes = createText.Notes,
                        Passcode = createText.Passcode,
                        StartDate = createText.StartDate,
                        EndDate = createText.EndDate
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

        public async Task<object> AddSchedule(string idUser, CreateSchedule createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                foreach (var item in items)
                {
                    if (item.typeWidget == "schedule")
                    {
                        // Define the filter by Id
                        var filter = Builders<AddLink>.Filter.Eq(_ => _.Id, item.Id);

                        // Define the update for the Content field
                        var update = Builders<AddLink>.Update.Set(_ => _.Content, new Content
                        {
                            Caption = createText.Caption,
                            WidgetSchedule = createText.ScheduleItem
                            .Select(id => new ScheduleItem
                            {
                                id = id.id,
                                Date = id.Date,
                                StartTime = id.StartTime,
                                EndTime = id.EndTime,
                            })
                            .ToList()
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
                    typeWidget = "schedule",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        Caption = createText.Caption,
                        WidgetSchedule = createText.ScheduleItem
                        .Select(id => new ScheduleItem
                        {
                            id = id.id,
                            Date = id.Date,
                            StartTime = id.StartTime,
                            EndTime = id.EndTime,
                        })
                        .ToList()
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

        public async Task<object> AddProfile(string idUser, CreateProfiles createText)
        {
            try
            {
                var items = await addLink.Find(_ => _.UserId == idUser).ToListAsync();
                long count = items.Count();

                foreach (var item in items)
                {
                    if (item.typeWidget == "profile")
                    {
                        // Define the filter by Id
                        var filter = Builders<AddLink>.Filter.Eq(_ => _.Id, item.Id);

                        // Define the update for the Content field
                        var update = Builders<AddLink>.Update.Set(_ => _.Content, new Content
                        {
                            FullName = createText.FullName,
                            Description = createText.Description,
                            UrlBanner = createText.UrlBanner,
                            UrlImageProfile = createText.UrlImageProfile,
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
                    typeWidget = "profile",
                    width = "100%",
                    CreatedAt = DateTime.Now,
                    Content = new Content
                    {
                        FullName = createText.FullName,
                        Description = createText.Description,
                        UrlBanner = createText.UrlBanner,
                        UrlImageProfile = createText.UrlImageProfile,
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

        public async Task<object> AddSosmed(string idUser, CreateSosmed createText)
        {
            try
            {
                // Define the filter using the UserId and Key
                var filter = Builders<AddSosmed>.Filter.And(
                    Builders<AddSosmed>.Filter.Eq(x => x.UserId, idUser),
                    Builders<AddSosmed>.Filter.Eq(x => x.Key, idUser + "/" + createText.Key)
                );

                // Check if the item exists
                var existingItem = await addSosmed.Find(filter).FirstOrDefaultAsync();

                if (existingItem != null)
                {
                    // Create the updated document
                    var updatedItem = new AddSosmed
                    {
                        Key = idUser + "/" + createText.Key,
                        Value = createText.Value,
                        UserId = idUser
                    };

                    // Update the existing item
                    await addSosmed.ReplaceOneAsync(filter, updatedItem);
                    return new { code = 200, message = "Berhasil Update" };
                }

                // If the item does not exist, insert a new document
                var newItem = new AddSosmed
                {
                    Key = idUser + "/" + createText.Key,
                    Value = createText.Value,
                    UserId = idUser
                };

                await addSosmed.InsertOneAsync(newItem);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {
                throw new CustomException(400, "Error", ex.Message);
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
                            WidgetCarouselAttachment = createText.attachmentIds
                            .Select(id => new CarouselItem
                            {
                                id = id,
                                widgetCarouselId = id,
                                attachmentId = id
                            })
                            .ToList()
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
                        WidgetCarouselAttachment = createText.attachmentIds
                        .Select(id => new CarouselItem
                        {
                            id = id,
                            widgetCarouselId = id,
                            attachmentId = id
                        })
                        .ToList()
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


        public async Task<object> ChangeWidth(string idUser, ChangeWidthDto createText)
        {
            try
            {
                var roleData = await addText.Find(x => x.Id.Equals(idUser, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync();
                if (roleData == null)
                    throw new CustomException(400, "Message", "ID not found");

                roleData.width = createText.width;
                await addText.ReplaceOneAsync(x => x.Id == roleData.Id, roleData);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> DeleteItem(string id)
        {
            try
            {
                await addLink.DeleteOneAsync(o => o.Id == id);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> DeleteSosmed(string idUser, string Key)
        {
            try
            {
                await addSosmed.DeleteOneAsync(o => o.Key == idUser + "/" + Key);
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

        public async Task<object> PostNewPos(List<UpdatePos> createText)
        {
            try
            {
                foreach (var updateDto in createText)
                {
                    var filter = Builders<AddLink>.Filter.Eq(item => item.Id, updateDto.Id);
                    var update = Builders<AddLink>.Update.Set(item => item.sequence, updateDto.sequence);

                    var result = await addLink.UpdateOneAsync(filter, update);
                }
                return new { code = 200, message = "Berhasil" };
            }
            catch (CustomException ex)
            {

                throw new CustomException(400, "Error", ex.Message); ;
            }
        }

    }
}