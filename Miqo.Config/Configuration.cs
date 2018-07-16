using System;

namespace Miqo.Config {
	/// <summary>
	/// A data type containing metadata about a configuration file.
	/// </summary>
	public class Configuration {
		public string Raw { get; set; }
		public object Parsed { get; set; }
		public bool FromFile { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
