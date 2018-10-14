using Newtonsoft.Json;

namespace Miqo.Config.Tests.ConfigClasses {
	public class ConfigurationWithIncorrectEncryption {
		[JsonConverter(typeof(EncryptedPropertyConverter), "snafu")]
		public string SnafuString { get; set; }
	}
}
