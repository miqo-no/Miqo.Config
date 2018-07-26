using Newtonsoft.Json;

namespace Miqo.Config.Tests.ConfigClasses {
	public class ConfigurationWithEncryption {
		[JsonConverter(typeof(EncryptedPropertyConverter), "06c98cb49446d5200e272e4aa61566261278e53f6dc73a95f211694451787842")]
		public string ConnectionString { get; set; }

		public string ServerName { get; set; } = "localhost";
		public int PortNumber { get; set; } = 80;

		[JsonIgnore]
		public string IgnoredVariable { get; set; }
	}
}
