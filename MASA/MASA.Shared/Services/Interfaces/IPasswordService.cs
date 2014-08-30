namespace MASA.Services.Interfaces
{
    using System;

    /// <summary>
    /// Implement on Pages or a UserControl to retrieve a Password from a PasswordBox (or similar).
    /// This way we can avoid storing the password as an object in memory.
    /// 
    /// You will have to enforce that only one IPasswordService can be registered to the 
    /// SimpleIoc at any one time.
    /// </summary>
    public interface IPasswordService
    {
        String GetPassword();

        event EventHandler PasswordChanged;
    }
}
