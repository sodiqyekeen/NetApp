namespace NetApp.Domain.Enums;

public enum SessionCategory : short
{
    Login = 1,
    PasswordReset = 2,
    EmailConfirmation = 3,
    PhoneConfirmation = 4,
    TwoFactorAuthentication = 5,
    PasswordlessLogin = 6,
    PasswordlessRegistration = 7,
    PasswordlessEmailConfirmation = 8,
    PasswordlessPhoneConfirmation = 9,
    PasswordlessTwoFactorAuthentication = 10,
    PasswordlessPasswordReset = 11,
    PasswordlessEmailChange = 12,
    PasswordlessPhoneChange = 13,
    PasswordlessEmailOrPhoneChange = 14,
    PasswordlessEmailOrPhoneConfirmation = 15,
    PasswordlessEmailOrPhoneLogin = 16,
    RefreshToken = 17,
}
