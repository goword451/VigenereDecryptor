namespace VigenereDecryptor.Services
{
    public class CypherService : ICypherService
    {
        private static readonly string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        public string Encrypt(string text, string key)
        {
            int i = 0;
            string result = "";
            foreach (char symbol in text)
            {
                char lowerSymbol = char.ToLower(symbol);
                if (alphabet.Contains(lowerSymbol))
                {
                    int j = alphabet.IndexOf(lowerSymbol);
                    int k = alphabet.IndexOf(char.ToLower(key[i++ % key.Length]));
                    result += char.IsUpper(symbol) ? alphabet[(j + k) % alphabet.Length].ToString().ToUpper() : alphabet[(j + k) % alphabet.Length].ToString();
                }
                else
                {
                    result += symbol;
                }
            }
            return result;
        }

        public string Decrypt(string text, string key)
        {
            int i = 0;
            string result = "";
            foreach (char symbol in text)
            {
                char lowerSymbol = char.ToLower(symbol);
                if (alphabet.Contains(lowerSymbol))
                {
                    int m = alphabet.IndexOf(lowerSymbol);
                    int n = alphabet.IndexOf(char.ToLower(key[i++ % key.Length]));
                    result += char.IsUpper(symbol) ? alphabet[(m - n + alphabet.Length) % alphabet.Length].ToString().ToUpper() : alphabet[(m - n + alphabet.Length) % alphabet.Length].ToString();
                }
                else
                {
                    result += symbol;
                }
            }
            return result;
        }
    }
}
