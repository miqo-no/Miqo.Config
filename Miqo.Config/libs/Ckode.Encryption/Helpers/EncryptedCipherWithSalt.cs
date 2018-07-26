using System;

namespace Ckode.Encryption.Helpers
{
	internal class EncryptedCipherWithSalt : EncryptedCipher
	{
		internal EncryptedCipherWithSalt() : base()
		{
		}

		internal EncryptedCipherWithSalt(byte[] bytes) : base()
		{
			var position = 0;

			var keysizeBytes = GetArrayPart(bytes, position);
			position += keysizeBytes.Length + 1;
			Keysize = BitConverter.ToInt32(keysizeBytes, 0);

			IV = GetArrayPart(bytes, position);
			position += IV.Length + 1;

			CryptoSalt = GetArrayPart(bytes, position);
			position += CryptoSalt.Length + 1;

			CipherPosition = position;
			Cipher = bytes.GetSubArray(position);
		}

		internal byte[] CryptoSalt { get; set; }

		internal override byte[] ToBytes()
		{
			var keysizeBytes = BitConverter.GetBytes(Keysize);
			return CombineArrays(keysizeBytes, IV, CryptoSalt, Cipher);
		}
	}
}