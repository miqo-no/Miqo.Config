using System;
using System.IO;

namespace Miqo.Config {
	public class ConfigurationManager {
		public string ConfigurationFileLocation { get; private set; }
		private Configuration _config = new Configuration();
		public Action<string> Log { get; set; }
		public Action<string, Exception> LogException { get; set; }

		public ConfigurationManager ApplicationSettings() {
			ConfigurationFileLocation = AppDomain.CurrentDomain.BaseDirectory;
			return this;
		}

		public ConfigurationManager ApplicationSettings(string directory) {
			ConfigurationFileLocation = Directory.Exists(directory) ? directory : AppDomain.CurrentDomain.BaseDirectory;
			return this;
		}

		public ConfigurationManager UserSettings(string applicationName) {
			if (string.IsNullOrWhiteSpace(applicationName)) throw new ArgumentNullException();

			ConfigurationFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);
			return this;
		}

		public T LoadConfigurationFromFile<T>(string fileName, bool useAbsolutePath = false) where T : new() {
			if (fileName == null) throw new ArgumentNullException();
			var file = useAbsolutePath ? fileName : Path.Combine(ConfigurationFileLocation, fileName);

			var fromFile = ReadConfigurationFromFile(file);
			_config.LastUpdated = fromFile.LastUpdated;
			_config.Raw = fromFile.Data;
			_config.FromFile = true;

			_config.Parsed = ParseConfiguration<T>(_config.Raw);

			if (_config.Parsed is T p) return p;

			try {
				return (T) Convert.ChangeType(_config.Parsed, typeof(T));
			}
			catch (InvalidCastException) {
				Log?.Invoke($"Miqo.Config: Unable to cast the finished configurations.");
				return default(T);
			}
		}

		public T LoadConfigurationFromString<T>(string s) where T : new() {
			_config.LastUpdated = DateTime.UtcNow;
			_config.Raw = s;
			_config.FromFile = false;

			_config.Parsed = ParseConfiguration<T>(_config.Raw);

			if (_config.Parsed is T p) return p;

			try {
				return (T) Convert.ChangeType(_config.Parsed, typeof(T));
			}
			catch (InvalidCastException) {
				Log?.Invoke($"Miqo.Config: Unable to cast the finished configurations.");
				return default(T);
			}
		}

		public ConfigurationManager SaveConfiguration(object config) {
			if (config == null) throw new ArgumentNullException();

			var s = Newtonsoft.Json.JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);

			if (_config.Raw == s) {
				Log?.Invoke($"Miqo.Config: No changes detected in configuration. Skipping...");
				return this;
			}

			_config.LastUpdated = DateTime.UtcNow;
			_config.Parsed = config;
			_config.Raw = s;
			return this;
		}

		public void ToFile(string file, bool useAbsolutePath = false) {
			file = useAbsolutePath ? file : Path.Combine(ConfigurationFileLocation, file);

			try {
				File.WriteAllText(file, _config.Raw, System.Text.Encoding.UTF8);
			}
			catch (Exception e) {
				LogException?.Invoke($"Miqo.Config: Couldn't write file {file}", e);
				throw new Exception("Miqo.Config could not write to file.", e);
			}

			Log?.Invoke($"Miqo.Config: Configuration saved to file");
		}

		public override string ToString() {
			return _config.Raw;
		}


		private T ParseConfiguration<T>(string configRaw) where T : new() {
			var config = default(T);

			if (configRaw == null) {
				Log?.Invoke($"Miqo.Config: Cannot deserialize null string for type {typeof(T).Name}");
			}
			else if (configRaw != string.Empty) {
				try {
					config = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(configRaw);
				}
				catch (Exception e) {
					LogException?.Invoke($"Miqo.Config: An exception occured while deserializing configuration", e);
				}
			}

			if (config != null) return config;

			Log?.Invoke($"Miqo.Config: Unable to deserialize string {configRaw} to type {typeof(T).Name}");
			return new T();
		}

		private ConfigurationItem ReadConfigurationFromFile(string file) {
			var item = new ConfigurationItem();
			var data = "";

			try {
				data = File.ReadAllText(file, System.Text.Encoding.UTF8);
			}
			catch (FileNotFoundException e) {
				LogException?.Invoke($"Miqo.Config: Could not read file: {file}", e);
			}

			item.Data = data;
			item.LastUpdated = new FileInfo(file).LastWriteTimeUtc;

			return item;
		}
	}
}