using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Miqo.Config {
	public static class StringCipher {
		public static string EncryptString(string text, string passPhrase) {
			var cipher =
				new Ckode.Encryption.AESWithPassword()
					.Encrypt(text, Encoding.UTF8, passPhrase);

			return cipher.ToHex();
		}

		public static string DecryptString(string cipher, string passPhrase) {
			return new Ckode.Encryption.AESWithPassword()
				.Decrypt(cipher.HexToBytes(), Encoding.UTF8, passPhrase);
		}

		public static string CreateRandomKey() {
			var key = new Ckode.Encryption.AES().GenerateKey();

			return key.ToHex();
		}
	}
}
