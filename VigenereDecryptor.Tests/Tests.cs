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

        
        

        [TestCase("������, ���!", "������", "�����, ���!")]
        [TestCase("��������", "�������", "��������")]
        [TestCase("����� ��� �������", "������", "����� ��� �������")]
        public void EncryptionTest(string input, string keyword, string output)
        {
            Assert.AreEqual(encryptor.Encryptor(input, keyword), output);
        }


        [TestCase("�����, ���!", "������", "������, ���!")]
        [TestCase("��������", "�������", "��������")]
        [TestCase("����� ��� �������", "������", "����� ��� �������")]
        public void DecryptionTest(string input, string keyword, string output)
        {
            Assert.AreEqual(encryptor.Decryptor(input, keyword), output);
        }
    }
}