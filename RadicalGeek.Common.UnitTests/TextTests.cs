using Microsoft.VisualStudio.TestTools.UnitTesting;
using RadicalGeek.Common.Text;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class TextTests
    {
        [TestMethod]
        public void CountOfNonOverlapping()
        {
            const string testString = "OXO OXO OXOXO";
            const string pattern = "OXO";
            Assert.AreEqual(3, testString.CountOf(pattern));
        }

        [TestMethod]
        public void CountOfOverlapping()
        {
            const string testString = "OXO OXO OXOXO";
            const string pattern = "OXO";
            Assert.AreEqual(4, testString.CountOf(pattern, true));
        }

        [TestMethod]
        public void DecryptTest()
        {
            // The repeated call is by design; RijndaelManaged says that its transform is reusable.
            Assert.AreEqual("Hello, World!", "FMZIZb4OymxeB0uPHTKcWA==".Decrypt());
            Assert.AreEqual("Hello, World!", "FMZIZb4OymxeB0uPHTKcWA==".Decrypt());
        }

        [TestMethod]
        public void DecryptReturnsPlainTextTest()
        {
            Assert.AreEqual("Hello, World!", "Hello, World!".Decrypt());
        }

        [TestMethod]
        public void CheckTruncateTruncates()
        {
            Assert.AreEqual("12345","1234567890".Truncate(5));
        }

        [TestMethod]
        public void MatchCharactersTest()
        {
            Assert.AreEqual("0123456789","0123456789".MatchCharacters(MatchType.Numeric));
            const string alphabet = "abcdefghijklmnopqrstuvwxyz";
            Assert.AreEqual(alphabet,alphabet.MatchCharacters(MatchType.Alphabetic));
            Assert.AreEqual("Hello World 123","Hello, World! 123".MatchCharacters(MatchType.Alphanumeric | MatchType.Space));
        }
    }
}
