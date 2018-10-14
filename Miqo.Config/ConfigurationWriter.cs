using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Miqo.Config.Formatters;

namespace Miqo.Config
{
    /// <summary>
    /// Implementation of a fluent interface to save a strongly typed
    /// configuration object to a configuration store.
    /// </summary>
    public class ConfigurationWriter : ICanSelectConfigurationStoreToSave, ICanSaveConfiguration
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
        private readonly object _configuration;

        /// <summary>
        /// The directory where the configuration file will be saved.
        /// </summary>
        private string _location;

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigurationWriter"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        /// <param name="formatter">The <see cref="IConfigurationFormatter"/> to use.</param>
        /// <param name="configuration">The configuration object to be serialized.</param>
        public ConfigurationWriter(ILogger logger, IConfigurationFormatter formatter, object configuration)
        {
            _logger = logger ?? new NullLogger<MiqoConfig>();
            _formatter = formatter;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Application wide configurations are saved in the same directory as
        /// the application itself.
        /// </summary>
        /// <returns></returns>
        public ICanSaveConfiguration ApplicationSettings()
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
        public ICanSaveConfiguration ApplicationSettings(string directory)
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
        public ICanSaveConfiguration UserSettings(string applicationName)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
                throw new ArgumentNullException(nameof(applicationName));

            _location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), applicationName);
            _logger.LogDebug($"Location: {_location}");

            return this;
        }

        /// <summary>
        /// Serializes the configuration object to a file.
        /// </summary>
        /// <param name="file">
        /// The name of the configuration file including extension name.
        /// </param>
        /// <param name="useAbsolutePath">
        /// Specifies if the <paramref name="file"/> is an absolute path.
        /// </param>

        public void ToFile(string file, bool useAbsolutePath = false)
        {
            if (string.IsNullOrWhiteSpace(file))
                throw new ArgumentNullException(nameof(file));

            file = useAbsolutePath ? file : Path.Combine(_location, file);

            try
            {
                File.WriteAllText(file, ToString(), Encoding.UTF8);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Couldn't write file {file}");
                throw new Exception("Miqo.Config could not write to file.", e);
            }

            _logger.LogInformation("Configuration saved to file");
        }

        /// <summary>
        /// Serializes the configuration to a <see cref="Stream"/>.
        /// </summary>
        /// <returns>The <see cref="Stream"/>.</returns>
        public Stream ToStream()
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(ToString() ?? ""));
        }

        /// <summary>
        /// Serializes the strongly typed configuration object to a string.
        /// </summary>
        /// <returns>The serialized string.</returns>
        public override string ToString()
        {
            return _formatter.Serialize(_configuration);
        }
    }
}
