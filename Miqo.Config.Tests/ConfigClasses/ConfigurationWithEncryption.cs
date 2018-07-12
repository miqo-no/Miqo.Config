using Newtonsoft.Json;

namespace Miqo.Config.Tests.ConfigClasses {
	public class ConfigurationWithEncryption {
		[JsonConverter(typeof(EncryptedPropertyConverter), "8ef51d43-03b9-4831-b415-5c73d472340d")]
		public string ConnectionString { get; set; }

		public string ServerName { get; set; } = "localhost";
		public int PortNumber { get; set; } = 80;

		[JsonIgnore]
		public string IgnoredVariable { get; set; }
	}
}