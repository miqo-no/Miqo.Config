using Newtonsoft.Json;

namespace Miqo.Config.Tests.ConfigClasses {
	public class ConfigurationWithEncryption {
		[JsonConverter(typeof(EncryptedPropertyConverter), "cfVMjtOJ8/eJx0037MHNym3awHj9iAUBdM/bmiLUvlc=")]
		public string ConnectionString { get; set; }

		public string ServerName { get; set; } = "localhost";
		public int PortNumber { get; set; } = 80;

		[JsonIgnore]
		public string IgnoredVariable { get; set; }
	}
}
