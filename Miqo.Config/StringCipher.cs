using System;
using System.Text;

namespace Miqo.Config {
	public static class StringCipher {
		public static string EncryptString(string text, string passPhrase) {
			var cipher =
				new Ckode.Encryption.AESWithPassword()
					.Encrypt(text, Encoding.UTF8, passPhrase);

			return Convert.ToBase64String(cipher);
		}

		public static string DecryptString(string cipher, string passPhrase) {
			// Check if cipher is stored as a hex encoded string instead of a base64 string.
			var bytes = StringContainsOnlyHex(cipher)
				? cipher.HexToBytes()
				: Convert.FromBase64String(cipher);

			return new Ckode.Encryption.AESWithPassword()
				.Decrypt(bytes, Encoding.UTF8, passPhrase);

		}

		public static string CreateRandomKey() {
			var key = new Ckode.Encryption.AES().GenerateKey();

			return Convert.ToBase64String(key);
		}

		public static bool StringContainsOnlyHex(string test)
			=> System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
	}
}
