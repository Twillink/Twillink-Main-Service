
public interface IAuthService
{
    Task<Object> LoginAsync(LoginDto model);
    Task<Object> RegisterAsync(RegisterDto model);
    Task<Object> Aktifasi(string id);
    Task<Object> UpdatePassword(string id, UpdatePasswordDto model);
    Task<Object> UpdatePin(string id, UpdatePinDto model);
    Task<Object> VerifyOtp(OtpDto otp);
    Task<Object> VerifyPin(string id);
    Task<Object> CheckPin(PinDto pin, string id);
    Task<Object> RequestOtpEmail(string id);

}