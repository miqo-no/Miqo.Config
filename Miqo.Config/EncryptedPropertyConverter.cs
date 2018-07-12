using System;
using Newtonsoft.Json;

namespace Miqo.Config {
	public class EncryptedPropertyConverter : JsonConverter {
		private readonly string _key;

		public EncryptedPropertyConverter(string encryptionKey) {
			if (encryptionKey != null)
				_key = encryptionKey;
			else
				throw new ArgumentNullException(nameof(encryptionKey));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
			var stringValue = (string) value;
			if (string.IsNullOrEmpty(stringValue)) {
				writer.WriteNull();
				return;
			}

			writer.WriteValue(StringCipher.Encrypt(stringValue, _key));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
			var value = reader.Value as string;
			if (string.IsNullOrEmpty(value)) {
				return reader.Value;
			}

			try {
				return StringCipher.Decrypt(value, _key);
			}
			catch {
				return string.Empty;
			}
		}

		/// <inheritdoc />
		public override bool CanConvert(Type objectType) {
			return objectType == typeof(string);
		}
	}
}