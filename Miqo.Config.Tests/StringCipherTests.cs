using System;
using System.Security.Cryptography;
using Xunit;

namespace Miqo.Config.Tests
{
    public class StringCipherTests
    {
        [Fact]
        public void EncryptString_CanEncryptAndDecryptString()
        {
            const string text = "100% fluffy goodness";
            const string phrase = "cfVMjtOJ8/eJx0037MHNym3awHj9iAUBdM/bmiLUvlc=";

            var cipher = StringCipher.EncryptString(text, phrase);
            var decrypted = StringCipher.DecryptString(cipher, phrase);

            Assert.Equal(text, decrypted);
        }

        [Fact]
        public void EncryptString_ThrowsException_WhenPassPhraseIsNull()
        {
            const string goodText = "100% fluffy goodness";
            const string badText = " ";
            const string goodPhrase = "cfVMjtOJ8/eJx0037MHNym3awHj9iAUBdM/bmiLUvlc=";
            const string badPhrase = null;

            Assert.Throws<ArgumentNullException>(
                () => StringCipher.EncryptString(goodText, badPhrase));

            Assert.Throws<ArgumentNullException>(
                () => StringCipher.EncryptString(badText, goodPhrase));
        }

        [Fact]
        public void DecryptString_ThrowsException_WhenPassPhraseIsNull()
        {
            const string goodText = "100% fluffy goodness";
            const string badText = null;
            const string goodPhrase = "cfVMjtOJ8/eJx0037MHNym3awHj9iAUBdM/bmiLUvlc=";
            const string badPhrase = null;

            Assert.Throws<ArgumentNullException>(
                () => StringCipher.DecryptString(goodText, badPhrase));

            Assert.Throws<ArgumentNullException>(
                () => StringCipher.DecryptString(badText, goodPhrase));
        }

        [Fact]
        public void DecryptString_ThrowsException_WhenPassPhraseIsNotBase64()
        {
            const string text = "100% fluffy goodness";
            const string phrase = "!incorrect!";

            Assert.Throws<FormatException>(
                () => StringCipher.DecryptString(text, phrase));
        }

        [Fact]
        public void DecryptString_ThrowsException_WhenIncorrectPassPhraseIsUsed()
        {
            const string text = "100% fluffy goodness";
            var phrase = StringCipher.CreateRandomKey();
            var cipher = StringCipher.EncryptString(text, phrase);

            var newPhrase = StringCipher.CreateRandomKey();

            Assert.Throws<CryptographicException>(
                () => StringCipher.DecryptString(cipher, newPhrase));
        }
    }
}
