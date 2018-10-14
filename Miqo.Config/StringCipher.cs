using System;
using System.Text;

namespace Miqo.Config
{
    /// <summary>
    /// A helper class to perform AES encryption and decryption.
    /// </summary>
    public static class StringCipher
    {
        /// <summary>
        /// Encrypt a string using AES.
        /// </summary>
        /// <param name="text">The string to encrypt.</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns>An AES encrypted cipher.</returns>
        public static string EncryptString(string text, string passPhrase)
        {
            if (text.IsNull()) throw new ArgumentNullException(nameof(text));
            if (passPhrase.IsNull()) throw new ArgumentNullException(nameof(passPhrase));

            var cipher =
                new Ckode.Encryption.AESWithPassword()
                    .Encrypt(text, Encoding.UTF8, passPhrase);

            return Convert.ToBase64String(cipher);
        }

        /// <summary>
        /// Decrypt a string using AES.
        /// </summary>
        /// <param name="cipher">The AES encrypted cipher.</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns>The decrypted string.</returns>
        public static string DecryptString(string cipher, string passPhrase)
        {
            if (cipher.IsNull()) throw new ArgumentNullException(nameof(cipher));
            if (passPhrase.IsNull()) throw new ArgumentNullException(nameof(passPhrase));

            // Check if cipher is stored as a hex encoded string instead of a base64 string.
            var bytes = StringContainsOnlyHex(cipher)
                ? cipher.HexToBytes()
                : Convert.FromBase64String(cipher);

            return new Ckode.Encryption.AESWithPassword()
                .Decrypt(bytes, Encoding.UTF8, passPhrase);
        }

        /// <summary>
        /// Helper method to create a random pass phrase.
        /// </summary>
        /// <returns>A pass phrase for use with AES encryption.</returns>
        public static string CreateRandomKey()
        {
            var key = new Ckode.Encryption.AES().GenerateKey();

            return Convert.ToBase64String(key);
        }

        /// <summary>
        /// Checks if a string is hex encoded.
        /// </summary>
        /// <param name="text">The string that should be checked.</param>
        /// <returns>Returns TRUE if the strong only contains hex characters.</returns>
        private static bool StringContainsOnlyHex(string text)
            => System.Text.RegularExpressions.Regex.IsMatch(text, @"\A\b[0-9a-fA-F]+\b\Z");
    }
}
