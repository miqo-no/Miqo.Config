using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Miqo.Config.Formatters;

namespace Miqo.Config
{
    /// <summary>
    /// Implementation of a fluent interface to load a strongly typed
    /// configuration object from a configuration store.
    /// </summary>
    public class ConfigurationReader : ICanSelectConfigurationStoreToLoad, ICanLoadConfiguration
    {
        /// <summary>
        /// The <see cref="ILogger"/> to use.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The <see cref="IConfigurationFormatter"/> to use.
        /// </summary>
        private readonly IConfigurationFormatter _formatter;
        
        /// <summary>
        /// The configuration object to be serialized.
        /// </summary>
        private string _location;

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationReader"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        /// <param name="formatter">The <see cref="IConfigurationFormatter"/> to use.</param>
        public ConfigurationReader(ILogger logger, IConfigurationFormatter formatter)
        {
            _logger = logger;
            _formatter = formatter;
        }

        /// <summary>
        /// Application wide configurations are saved in the same directory as
        /// the application itself.
        /// </summary>
        /// <returns></returns>
        public ICanLoadConfiguration ApplicationSettings()
        {
            _location = AppDomain.CurrentDomain.BaseDirectory;
            _logger.LogDebug($"Location: {_location}");

            return this;
        }

        /// <summary>
        /// Application wide configurations are saved in the specified
        /// directory.
        /// </summary>
        /// <param name="directory">The directory to use.</param>
        /// <returns></returns>
        public ICanLoadConfiguration ApplicationSettings(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory))
                throw new ArgumentNullException(nameof(directory));

            _location = Directory.Exists(directory) ? directory : AppDomain.CurrentDomain.BaseDirectory;
            _logger.LogDebug($"Location: {_location}");

            return this;
        }

        /// <summary>
        /// User specific configurations are saved in the currently logged in
        /// user's roaming application data folder.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the application can be used as a subfolder in the
        /// roaming application data folder.
        /// </param>
        /// <returns></returns>
        public ICanLoadConfiguration UserSettings(string applicationName)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentNullException(nameof(applicationName));

            _location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);
            _logger.LogDebug($"Location: {_location}");

            return this;
        }

        /// <summary>
        /// Deserializes a file to a strongly typed configuration object.
        /// </summary>
        /// <typeparam name="T">The strongly typed configuration object.</typeparam>
        /// <param name="file">
        /// The name of the configuration file including extension name.
        /// </param>
        /// <param name="useAbsolutePath">
        /// Specifies if the <paramref name="file"/> is an absolute path.
        /// </param>
        /// <returns>
        /// A strongly typed configuration object.
        /// </returns>
        public T FromFile<T>(string file, bool useAbsolutePath = false) where T : new()
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException(nameof(file));

            var data = string.Empty;
            file = useAbsolutePath ? file : Path.Combine(_location, file);

            try
            {
                data = File.ReadAllText(file, Encoding.UTF8);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to read configuration file.");
            }
            
            return FromString<T>(data);
        }

        /// <summary>
        /// Deserializes a <see cref="Stream"/> to a strongly typed
        /// configuration object.
        /// </summary>
        /// <typeparam name="T">The strongly typed configuration object.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> to deserialize.</param>
        /// <returns>
        /// A strongly typed configuration object.
        /// </returns>
        public T FromStream<T>(Stream stream) where T : new()
        {
            var data = string.Empty;
            
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                data = reader.ReadToEnd();
            }

            return FromString<T>(data);
        }

        /// <summary>
        /// Deserializes a <see cref="string"/> to a strongly typed
        /// configuration object.
        /// </summary>
        /// <typeparam name="T">The strongly typed configuration object.</typeparam>
        /// <param name="data">The <see cref="string"/> to deserialize.</param>
        /// <returns>A strongly typed configuration object.</returns>
        public T FromString<T>(string data) where T : new()
        {
            return _formatter.Parse<T>(data);
        }
    }
}
