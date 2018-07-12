using System;

namespace Miqo.Config {
	public class Configuration {
		public string Raw { get; set; }
		public object Parsed { get; set; }
		public bool FromFile { get; set; }
		public DateTime LastUpdated { get; set; }
	}
}
