using NUnit.Framework;
using VigenereDecryptor.Services;

namespace VigenereDecryptor.Tests
{
    [TestFixture]
    public class Tests
    {
        //[SetUp]
        public Tests (ICypher encryptor)
        {
            this.encryptor = encryptor;
        }
        
        private ICypher encryptor;

        
        

        [TestCase("Привет, мир!", "дружба", "Убьиёт, рщд!")]
        [TestCase("Скорпион", "Сабзиро", "Гкпшшщэя")]
        [TestCase("Кодим код красиво", "Дотнэт", "Оэццй этт эюэдмрб")]
        public void EncryptionTest(string input, string keyword, string output)
        {
            Assert.AreEqual(encryptor.Encryptor(input, keyword), output);
        }


        [TestCase("Убьиёт, рщд!", "дружба", "Привет, мир!")]
        [TestCase("Гкпшшщэя", "Сабзиро", "Скорпион")]
        [TestCase("Оэццй этт эюэдмрб", "Дотнэт", "Кодим код красиво")]
        public void DecryptionTest(string input, string keyword, string output)
        {
            Assert.AreEqual(encryptor.Decryptor(input, keyword), output);
        }
    }
}