using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Miqo.Config {
	/// <summary>
	/// The Configuration Manager manages either application wide or user specific settings
	/// </summary>
	[Obsolete("Deprecated. Use MiqoConfig class instead.")]
	public class ConfigurationManager {
		public string ConfigurationFileLocation { get; private set; }
		private Configuration _config = new Configuration();
		public Action<string> Log { get; set; }
		public Action<string, Exception> LogException { get; set; }

		/// <summary>
		/// Manage application wide settings, storing the configuration file in the same location as the application
		/// </summary>
		/// <returns></returns>
		public ConfigurationManager ApplicationSettings() {
			ConfigurationFileLocation = AppDomain.CurrentDomain.BaseDirectory;
			return this;
		}

		/// <summary>
		/// Manage application wide settings, storing the configuration file in a custom directory
		/// </summary>
		/// <param name="directory">Location of configuration file</param>
		/// <returns></returns>
		public ConfigurationManager ApplicationSettings(string directory) {
			ConfigurationFileLocation = Directory.Exists(directory) ? directory : AppDomain.CurrentDomain.BaseDirectory;
			return this;
		}

		/// <summary>
		/// Manage user specific settings, storing the configuration file in the user'json Application Data system directory.
		/// </summary>
		/// <remarks>
		/// The configuration file should be placed in a subfolder specific to your application.
		/// </remarks>
		/// <param name="applicationName">Name of the application</param>
		/// <returns></returns>
		public ConfigurationManager UserSettings(string applicationName) {
			if (string.IsNullOrWhiteSpace(applicationName)) throw new ArgumentNullException();

			ConfigurationFileLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);
			return this;
		}

		/// <summary>
		/// Load a configuration file, parses the JSON contents of the file, and returns a strongly typed object 
		/// </summary>
		/// <typeparam name="T">Strongly typed object with your application'json configuration properties</typeparam>
		/// <param name="file">Name of the configuration file</param>
		/// <param name="useAbsolutePath">Set to true if <paramref name="file"/> is an absolute path</param>
		/// <returns></returns>
		public T LoadConfigurationFromFile<T>(string file, bool useAbsolutePath = false) where T : new() {
			if (file == null) throw new ArgumentNullException();
			file = useAbsolutePath ? file : Path.Combine(ConfigurationFileLocation, file);

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

		/// <summary>
		/// Load a configuration from a JSON string, parses the JSON, and returns a strongly typed object 
		/// </summary>
		/// <typeparam name="T">Strongly typed object with your application'json configuration properties</typeparam>
		/// <param name="json">String containing the configuration formatted as JSON</param>
		/// <returns></returns>
		public T LoadConfigurationFromString<T>(string json) where T : new() {
			_config.LastUpdated = DateTime.UtcNow;
			_config.Raw = json;
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

		/// <summary>
		/// Prepares the configuration for being saved to a file or a string
		/// </summary>
		/// <remarks>
		/// Use <see cref="ToFile(string, bool)"/> to save the configuration as a file,
		/// or <see cref="ToString"/> to save the configuration as a string
		/// </remarks>
		/// <param name="config">Your strongly typed configuration class</param>
		/// <returns></returns>
		public ConfigurationManager SaveConfiguration(object config) {
			if (config == null) throw new ArgumentNullException();

			var settings = new JsonSerializerSettings {
				ContractResolver = new DefaultContractResolver {NamingStrategy = new CamelCaseNamingStrategy()},
				Formatting = Formatting.Indented
			};
			var s = JsonConvert.SerializeObject(config, settings);

			if (_config.Raw == s) {
				Log?.Invoke($"Miqo.Config: No changes detected in configuration. Skipping...");
				return this;
			}

			_config.LastUpdated = DateTime.UtcNow;
			_config.Parsed = config;
			_config.Raw = s;
			return this;
		}

		/// <summary>
		/// Saves the configuration as a JSON file
		/// </summary>
		/// <param name="file">Name of the configuration file</param>
		/// <param name="useAbsolutePath">Set to true if <paramref name="file"/> is an absolute path</param>
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

		/// <summary>
		/// Returns the configuration as a JSON formatted string
		/// </summary>
		/// <returns>JSON formatted string</returns>
		public override string ToString() {
			return _config.Raw;
		}

		/// <summary>
		/// Parses a JSON formatted string and returns a strongly typed configuration class
		/// </summary>
		/// <typeparam name="T">Strongly typed configuration class</typeparam>
		/// <param name="configRaw">JSON formatted string</param>
		/// <returns></returns>
		private T ParseConfiguration<T>(string configRaw) where T : new() {
			var config = default(T);

			if (configRaw == null) {
				Log?.Invoke($"Miqo.Config: Cannot deserialize null string for type {typeof(T).Name}");
			}
			else if (configRaw != string.Empty) {
				try {
					config = JsonConvert.DeserializeObject<T>(configRaw);
				}
				catch (Exception e) {
					LogException?.Invoke($"Miqo.Config: An exception occured while deserializing configuration", e);
				}
			}

			if (config != null) return config;

			Log?.Invoke($"Miqo.Config: Unable to deserialize string {configRaw} to type {typeof(T).Name}");
			return new T();
		}

		/// <summary>
		/// Reads a configuration file, and returns the contents as a string
		/// </summary>
		/// <param name="file">Location of JSON configuration file</param>
		/// <returns></returns>
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
