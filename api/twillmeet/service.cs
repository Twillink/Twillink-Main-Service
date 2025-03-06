using System.Globalization;
using MongoDB.Driver;
using Twillink.Shared.Models;

namespace RepositoryPattern.Services.TwilmeetService
{
    public class TwilmeetService : ITwilmeetService
    {
        private readonly IMongoCollection<Twilmeet> dataUser;
        private readonly IMongoCollection<Payment> dataPayment;
        private readonly IMongoCollection<User> dataListUser;

        private readonly IWidgetService _IWidgetService;


        private readonly string key;

        public TwilmeetService(IConfiguration configuration, IWidgetService roleService)
        {
            _IWidgetService = roleService;
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("Twillink");
            dataUser = database.GetCollection<Twilmeet>("Twilmeets");
            dataPayment = database.GetCollection<Payment>("Payments");
            dataListUser = database.GetCollection<User>("Users");


            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }

        public async Task<object> Get()
        {
            try
            {
                // Fetch all active users
                var activeUsers = await dataUser.Find(_ => _.IsActive == true).ToListAsync();
                // Prepare a dictionary to group payments by user
                var groupedPayments = new Dictionary<string, List<Payment>>();
                var groupedNonPayments = new Dictionary<string, List<Payment>>();


                // Iterate over active users
                foreach (var user in activeUsers)
                {
                    // Fetch payments for the current user
                    var payments = await dataPayment.Find(payment => payment.IdItem == user.Id && payment.IsVerification == true).ToListAsync();
                    var payments2 = await dataPayment.Find(payment => payment.IdItem == user.Id && payment.IsVerification == false).ToListAsync();

                    // Add user and their payments to the dictionary
                    groupedPayments[user.Id] = payments;
                    groupedNonPayments[user.Id] = payments2;

                }

                // Create the response object
                var response = activeUsers.Select(user => new
                {
                    Id = user.Id,
                    Price = user.Price, // Adjust the field based on your model
                    Owner = user.IdUser,
                    InfoItem = user,
                    Member = groupedPayments.ContainsKey(user.Id) ? groupedPayments[user.Id] : new List<Payment>(),
                    NonMember = groupedNonPayments.ContainsKey(user.Id) ? groupedNonPayments[user.Id] : new List<Payment>()
                });
                // Return response with the collected results
                return new { code = 200, data = response, message = "Data fetched successfully." };
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return new { code = 500, data = (object)null, message = $"Error: {ex.Message}" };
            }
        }


        public async Task<Object> GetById(string id)
        {
            try
            {
                var items = await dataUser.Find(_ => _.Id == id).FirstOrDefaultAsync();
                var payments = await dataPayment.Find(payment => payment.IdItem == id && payment.IsVerification == true).ToListAsync();
                return new { code = 200, data = items, message = "Data Add Complete", infoMember = payments };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<Object> GetMemberWebinar(string id)
        {
            try
            {
                var payments = await dataPayment.Find(payment => payment.IdItem == id).ToListAsync();
                return new { code = 200, data = payments, message = "Data Add Complete"};
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> Post(CreateTwilmeetDto item, string idUser)
        {
            try
            {
                var guid = Guid.NewGuid().ToString();
                var TwilmeetData = new Twilmeet()
                {
                    Id = guid,
                    IdUser = idUser,
                    Type = item.Type,
                    IsPaid = item.IsPaid,
                    Price = item.Price,
                    IsActive = true,
                    Thumbnail = item.Thumbnail,
                    Title = item.Title,
                    Desc = item.Desc,
                    Date = item.Date,
                    Time = item.Time,
                    Category = item.Category,
                    Languange = item.Languange,
                    Tags = item.Tags,
                    IsCertificate = item.IsCertificate,
                    IsClass = item.IsClass,
                    Classes = item.Classes,
                    IsVerification = item.IsPaid == true ? false : true,
                    CreatedAt = DateTime.Now
                };

                var createWidget = new CreateWebinar()
                {
                    Title = item.Title,
                    UrlWebinar = guid.ToString(),
                    UrlThumbnail = item.Thumbnail,
                    Description = item.Desc,
                    Notes = item.Price.ToString(),
                    Passcode = "",
                    StartDate = ParseDateTime(item.Date),
                    EndDate = ParseDateTime(item.Time),
                };
                await dataUser.InsertOneAsync(TwilmeetData);
                await _IWidgetService.AddWebinar(idUser, createWidget);
                return new { code = 200, id = TwilmeetData.Id, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        private DateTime? ParseDateTime(string dateString)
        {
            if (string.IsNullOrWhiteSpace(dateString))
                return null;

            if (DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                return parsedDate;
            }

            return null; // Return null if parsing fails
        }

        public async Task<object> PostBuy(CreateBuyTwilmeetDto item)
        {
            try
            {
                var check = await dataUser.Find(x => x.Id == item.IdItem).FirstOrDefaultAsync();
                if (check == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }

                var check2 = await dataPayment.Find(x => x.Email == item.Email && x.Id == item.IdItem).FirstOrDefaultAsync();
                if (check2 != null)
                {
                    throw new CustomException(400, "Error", "Data found, Please use another email");
                }
                // if (check.IsPaid == true && item.Price < check.Price)
                // {
                //     throw new CustomException(400, "Error", "Not Have Money");
                // }

                // var checkUser = await dataListUser.Find(x => x.Id == idUser).FirstOrDefaultAsync();
                // if (checkUser == null)
                // {
                //     throw new CustomException(400, "Error", "Data Not Found");
                // }

                var TwilmeetData = new Payment()
                {
                    Id = Guid.NewGuid().ToString(),
                    // IdUser = idUser,
                    TypePayment = item.TypePayment,
                    IdItem = item.IdItem,
                    Price = item.Price,
                    Photo = "",
                    NameUser = item.FirstName,
                    LastUser = item.LastName,
                    Phone = item.Phone,
                    Email = item.Email,
                    PasswordRoom = GenerateRandomText(6),
                    IsActive = true,
                    IsVerification = false,
                    CreatedAt = DateTime.Now
                };
                await dataPayment.InsertOneAsync(TwilmeetData);
                return new { code = 200, id = TwilmeetData.Id, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> PostApproval(string id)
        {
            try
            {
                var TwilmeetData = await dataPayment.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (TwilmeetData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                TwilmeetData.IsVerification = true;
                await dataPayment.ReplaceOneAsync(x => x.Id == id, TwilmeetData);
                return new { code = 200, id = TwilmeetData.Id.ToString(), message = "Data Updated" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> PostDecline(string id)
        {
            try
            {
                var TwilmeetData = await dataPayment.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (TwilmeetData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                TwilmeetData.IsActive = false;
                TwilmeetData.IsVerification = false;
                await dataPayment.ReplaceOneAsync(x => x.Id == id, TwilmeetData);
                return new { code = 200, id = TwilmeetData.Id.ToString(), message = "Data Updated" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public static string GenerateRandomText(int length)
        {
            return Guid.NewGuid().ToString("N").Substring(0, length);
        }

        public async Task<object> Put(string id, CreateTwilmeetDto item)
        {
            try
            {
                var TwilmeetData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (TwilmeetData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                TwilmeetData.Thumbnail = item.Thumbnail;
                await dataUser.ReplaceOneAsync(x => x.Id == id, TwilmeetData);
                return new { code = 200, id = TwilmeetData.Id.ToString(), message = "Data Updated" };
            }
            catch (CustomException)
            {
                throw;
            }
        }
        public async Task<object> Delete(string id)
        {
            try
            {
                var TwilmeetData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync();
                if (TwilmeetData == null)
                {
                    throw new CustomException(400, "Error", "Data Not Found");
                }
                TwilmeetData.IsActive = false;
                await dataUser.ReplaceOneAsync(x => x.Id == id, TwilmeetData);
                return new { code = 200, id = TwilmeetData.Id.ToString(), message = "Data Deleted" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> CreateLinkMeet(string idUser)
        {
            try
            {
                // Call ZoomService to create the meeting
                var zoomService = new ZoomService();
                var meetingDetails = await zoomService.CreateLinkMeet(idUser);

                // Return the meeting details as an object
                return meetingDetails;
            }
            catch (Exception ex)
            {
                // Handle and log the error
                throw new CustomException(500, "Error Creating Zoom Meeting", ex.Message);
            }
        }

    }
}