using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Miqo.Config.Tests.ConfigClasses {
	public class ConfigurationWithIncorrectEncryption {
		[JsonConverter(typeof(EncryptedPropertyConverter), "snafu")]
		public string SnafuString { get; set; }
	}
}