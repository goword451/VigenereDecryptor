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

        [TestCase("������, ���!", "������", "�����, ���!")]
        [TestCase("��������", "�������", "��������")]
        [TestCase("����� ��� �������", "������", "����� ��� �������")]
        public void EncryptionTest(string input, string keyword, string output)
        {
            Assert.AreEqual(CypherService.Encrypt(input, keyword), output);
        }

        [TestCase("�����, ���!", "������", "������, ���!")]
        [TestCase("��������", "�������", "��������")]
        [TestCase("����� ��� �������", "������", "����� ��� �������")]
        public void DecryptionTest(string input, string keyword, string output)
        {
            Assert.AreEqual(CypherService.Decrypt(input, keyword), output);
        }
    }
}