using ConDep.Console.Encrypt;
using ConDep.Dsl.Security;
using NUnit.Framework;

namespace ConDep.Console.Tests
{
    [TestFixture]
    public class Encryption
    {
        [Test]
        public void TestThat_Parse()
        {
            var parser = new CmdEncryptParser(new string[] { });
            var options = parser.Parse();
        }

        //[Test]
        //public void TestThat_ParseJsonFile()
        //{
        //    var handler = new CmdEncryptHandler(new string[0]);
        //    handler.Execute(null, null);
        //}

        [Test]
        public void TestThat()
        {
            var key = JsonPasswordCrypto.GenerateKey(256);
            var crypto = new JsonPasswordCrypto(key);

            var somePass = "someW�irdP@ssw0rdWith���";
            var encryptedPassword = crypto.Encrypt(somePass);

            var decrPasswd = crypto.Decrypt(encryptedPassword);

            Assert.That(somePass, Is.EqualTo(decrPasswd));
        }
    }
}