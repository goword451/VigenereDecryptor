namespace VigenereDecryptor.Services
{
    public interface ICypher
    {
        string Encryptor(string text, string key);
        string Decryptor(string text, string key);
    }
}
