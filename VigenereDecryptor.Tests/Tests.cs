using NUnit.Framework;
using VigenereDecryptor.Services;

namespace VigenereDecryptor.Tests
{
    [TestFixture]
    public class CypherServiceTests
    {
        private ICypherService CypherService { get; set; }

        [SetUp]

        public void SetUp()
        {
            CypherService = new CypherService();
        }

        [TestCase("Привет, мир!", "дружба", "Убьиёт, рщд!")]
        [TestCase("Скорпион", "Сабзиро", "Гкпшшщэя")]
        [TestCase("Кодим код красиво", "Дотнэт", "Оэццй этт эюэдмрб")]
        public void EncryptionTest(string input, string keyword, string output)
        {
            Assert.AreEqual(CypherService.Encrypt(input, keyword), output);
        }

        [TestCase("Убьиёт, рщд!", "дружба", "Привет, мир!")]
        [TestCase("Гкпшшщэя", "Сабзиро", "Скорпион")]
        [TestCase("Оэццй этт эюэдмрб", "Дотнэт", "Кодим код красиво")]
        public void DecryptionTest(string input, string keyword, string output)
        {
            Assert.AreEqual(CypherService.Decrypt(input, keyword), output);
        }
    }
}