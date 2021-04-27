using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

//[assembly: InternalsVisibleTo("VigenereDecryptor.Tests")]
namespace VigenereDecryptor.Services
{
    public class Cypher : ICypher
    {
        private static readonly string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";

        public string Encryptor(string text, string key)
        {
            int i = 0;
            string result = "";
            foreach (char sym in text)
            {
                char lowerSym = char.ToLower(sym);
                if (alphabet.Contains(lowerSym))
                {
                    int p = alphabet.IndexOf(lowerSym);
                    int k = alphabet.IndexOf(char.ToLower(key[i++ % key.Length]));
                    result += char.IsUpper(sym) ? alphabet[(p + k) % alphabet.Length].ToString().ToUpper()
                                                : alphabet[(p + k) % alphabet.Length].ToString();
                }
                else
                    result += sym;
            }
            return result;
        }

        public string Decryptor(string text, string key)
        {
            int i = 0;
            string result = "";
            foreach (char sym in text)
            {
                char lowerSym = char.ToLower(sym);
                if (alphabet.Contains(lowerSym))
                {
                    int c = alphabet.IndexOf(lowerSym);
                    int k = alphabet.IndexOf(char.ToLower(key[i++ % key.Length]));
                    result += char.IsUpper(sym) ? alphabet[(c - k + alphabet.Length) % alphabet.Length].ToString().ToUpper()
                                                : alphabet[(c - k + alphabet.Length) % alphabet.Length].ToString();
                }
                else
                    result += sym;
            }
            return result;
        }
    }
}
