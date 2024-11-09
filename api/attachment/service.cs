using Google.Cloud.Storage.V1;
using MongoDB.Driver;
using Twillink.Shared.Models;

namespace RepositoryPattern.Services.AttachmentService
{
    public class AttachmentService : IAttachmentService
    {
        private readonly string bucketName = "twillink";
        private readonly StorageClient storageClient;
        private readonly IMongoCollection<Attachments> AttachmentLink;
        private readonly IMongoCollection<User> users;

        private readonly string key;

        public AttachmentService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("twillink");
            AttachmentLink = database.GetCollection<Attachments>("Attachment");
            var credentialPath = configuration["GoogleCloud:CredentialsPath"];
            var googleCredential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(credentialPath);
            storageClient = StorageClient.Create(googleCredential);
            this.key = configuration.GetSection("AppSettings")["JwtKey"];
        }
        public async Task<Object> Get(string Username)
        {
            try
            {
                // var items = await AttachmentLink.Find(_ => _.Username.ToLower() == Username.ToLower()).FirstOrDefaultAsync();
                var Attachment = new Attachments
                {
                };
                return new { code = 200, data = Attachment, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<(string FileName, string Url)> Upload(IFormFile file, string fileName)
        {
            var bucket = bucketName;
            var folderName = "uploads";


            // Baca file sebagai stream
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Tentukan metadata file
            var contentType = file.ContentType;

            // Unggah file ke Google Cloud Storage
            var gcsFile = storageClient.UploadObject(bucket, fileName, contentType, memoryStream);

            // URL akses file
            var url = $"https://storage.googleapis.com/{bucket}/{fileName}";

            return (gcsFile.Name, url);
        }
    }
}