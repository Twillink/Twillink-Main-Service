using System.Net;
using System.Net.Mail;

namespace SendingEmail
{
    public class EmailService : IEmailService
    {
        private readonly string Email;
        private readonly string PW;
        private readonly string SMTP;
        private readonly string Port;
        public EmailService(IConfiguration configuration)
        {
            // this.Email = configuration.GetSection("EmailSmtp")["Email"];
            // this.PW = configuration.GetSection("EmailSmtp")["PW"];
            // this.SMTP = configuration.GetSection("EmailSmtp")["SMTP"];
            // this.Port = configuration.GetSection("EmailSmtp")["Port"];
        }
        public async Task SendEmailAsync(EmailForm model)
        {
            try
            {
                var email = "admin@coderchamps.co.id";
                var password = "hgzzbtjqvbbovjvr";

                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    Credentials = new NetworkCredential(email, password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = model.Subject,
                    Body = SelectorMessage(model),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(model.Email);

                await client.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent successfully.");
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"SMTP Exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
        private string SelectorMessage(EmailForm models)
        {
            if (models.Message == "Activation")
            {
                return SubmitMessage(models.Id);
            }
            if (models.Message == "OTP")
            {
                return OtpMessage(models.Otp);
            }
            if (models.Message == "BuyWebinar")
            {
                return PostBuyMessage(models);
            }
            if (models.Message == "ApprovalWebinar")
            {
                return PostApprovalMessage(models);
            }
            if (models.Message == "DeclineWebinar")
            {
                return PostDeclineMessage(models);
            }
            return "";
        }

        private String SubmitMessage(string id)
        {
            return $@"
                <div style='font-size:14px; font-family:Arial, sans-serif; color:#434343; line-height:1.5;'>
                    <p>Hello,</p>
                    <p>Thank you for registering with us! To complete your registration and activate your account, please click the link below:</p>
                    <p><a href='https://twillink-main-service-158474999909.asia-southeast2.run.app/api/v1/auth/activation/{id}' style='color:#1a73e8; text-decoration:none;'>Activate Your Account</a></p>
                    <p>If you did not request this activation, you can ignore this email.</p>
                    <p>Best regards,<br>Twillink Team</p>
                </div>";
        }

        private String PostBuyMessage(EmailForm data)
        {
            return $@"
                <div style='font-size:14px; font-family:Arial, sans-serif; color:#434343; line-height:1.5;'>
                    <p>Hello,</p>
                    <p>Thank you for registering event ( {data.Title} ) from {data.NameUser} , Please wait for admin approval. We will send the event URL shortly.</p>
                    <p>If you did not request this registration, you can ignore this email.</p>
                    <p>Best regards,<br>Twillink Team</p>
                </div>";
        }

        private String PostApprovalMessage(EmailForm data)
        {
            return $@"
                <div style='font-size:14px; font-family:Arial, sans-serif; color:#434343; line-height:1.5;'>
                    <p>Hello,</p>
                    <p>Thank you for joining event ( {data.Title} ) from {data.NameUser} , please click the link below:</p>
                    <p><a href='{data.URL}' style='color:#1a73e8; text-decoration:none;'>Join Webinar Now</a></p>
                    <p>Best regards,<br>Twillink Team</p>
                </div>";
        }

        private String PostDeclineMessage(EmailForm data)
        {
            return $@"
                <div style='font-size:14px; font-family:Arial, sans-serif; color:#434343; line-height:1.5;'>
                    <p>Hello,</p>
                    <p>Admin cancel your joining event ( {data.Title} ) from {data.NameUser} , please click the link below for re-joining event:</p>
                    <p><a href='{data.URL}' style='color:#1a73e8; text-decoration:none;'>Re-Join Event</a></p>
                    <p>Best regards,<br>Twillink Team</p>
                </div>";
        }

        private String OtpMessage(string id)
        {
            return $@"
                <div style='font-size:14px; font-family:Arial, sans-serif; color:#434343; line-height:1.5;'>
                    <p>Hello,</p>
                    <p>Thank you for choosing our service! Your One-Time Password (OTP) for account verification is:</p>
                    <p style='font-size:18px; font-weight:bold; color:#d32f2f;'>{id}</p>
                    <p>Please enter this OTP in the required field to complete the verification process. This OTP is valid for a limited time, so make sure to use it promptly.</p>
                    <p>If you did not request this OTP, please disregard this message.</p>
                    <p>Best regards,<br>Twillink Team</p>
                </div>";
        }

    }


    public class EmailForm
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Subject { get; set; }
        public string? Message { get; set; }
        public string? Otp { get; set; }
        public string? Password { get; set; }
        public string? Title { get; set; }
        public string? URL { get; set; }
        public string? NameUser { get; set; }

    }
}

// {
//   "email": "hilmanzutech@gmail.com",
//   "username": "hil",
//   "password": "padang123",
//   "confirmPassword": "padang123",
//   "localIANATimeZone": "string"
// }