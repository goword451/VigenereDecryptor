namespace VigenereDecryptor.Services
{
    public interface ICypherService
    {
        string Encrypt(string text, string key);

        string Decrypt(string text, string key);
    }
}
