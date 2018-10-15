using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Miqo.Config.Formatters;

namespace Miqo.Config
{
    /// <summary>
    /// Represents a configuration manager. Acts as an entry point to the
    /// Miqo.Config library.
    /// </summary>
    public class MiqoConfig
    {
        private readonly ILogger _logger;
        private readonly IConfigurationFormatter _formatter;

        /// <summary>
        /// Initializes a new <see cref="MiqoConfig"/> instance using the
        /// default <see cref="JsonConfigurationFormatter"/>.
        /// </summary>
        public MiqoConfig()
        {
            _logger = new NullLogger<MiqoConfig>();
            _formatter = new JsonConfigurationFormatter(_logger);
        }

        /// <summary>
        /// Initializes a new <see cref="MiqoConfig"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public MiqoConfig(ILogger logger)
        {
            _logger = logger;
            _formatter = new JsonConfigurationFormatter(_logger);
        }

        /// <summary>
        /// Initializes a new <see cref="MiqoConfig"/> instance.
        /// </summary>
        /// <param name="formatter">The <see cref="IConfigurationFormatter"/> to use.</param>
        public MiqoConfig(IConfigurationFormatter formatter)
        {
            _logger = new NullLogger<MiqoConfig>();
            _formatter = formatter;
        }

        /// <summary>
        /// Initializes a new <see cref="MiqoConfig"/> instance.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        /// <param name="formatter">The <see cref="IConfigurationFormatter"/> to use.</param>
        public MiqoConfig(ILogger logger, IConfigurationFormatter formatter)
        {
            _logger = logger;
            _formatter = formatter;
        }

        /// <summary>
        /// Provides a fluent interface for loading a configuration file.
        /// </summary>
        public ICanSelectConfigurationStoreToLoad Load()
            => new ConfigurationReader(_logger, _formatter);

        /// <summary>
        /// Provides a fluent interface for saving a strongly typed
        /// configuration object to a file.
        /// </summary>
        /// <param name="configuration">The strongly typed configuration object.</param>
        public ICanSelectConfigurationStoreToSave Save(object configuration)
            => new ConfigurationWriter(_logger, _formatter, configuration);
    }
}
