using System;

namespace Miqo.Config {
	/// <summary>
	/// A data type containing metadata about a configuration item.
	/// </summary>
	internal class ConfigurationItem {
		public string Data { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
