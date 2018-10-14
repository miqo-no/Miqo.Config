using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Miqo.Config.Formatters
{
    /// <summary>
    /// A JSON configuration formatter implementation.
    /// </summary>
    public class JsonConfigurationFormatter : IConfigurationFormatter
    {
        /// <summary>
        /// The <see cref="ILogger"/> instance to use.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="JsonConfigurationFormatter"/>.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public JsonConfigurationFormatter(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Parse a JSON formatted string to create a strongly typed
        /// configuration object.
        /// </summary>
        /// <typeparam name="T">Strongly typed configuration object.</typeparam>
        /// <param name="data">The JSON formatted string to parse.</param>
        /// <returns>A strongly typed configuration object.</returns>
        public T Parse<T>(string data) where T : new()
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                _logger.LogWarning($"Cannot deserialize null string for type {typeof(T).Name}");
                return new T();
            }

            var config = default(T);

            try
            {
                config = JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An exception occured while deserializing configuration");
            }

            if (config != null) return config;

            _logger.LogWarning($"Unable to deserialize string to type {typeof(T).Name}");
            return new T();
        }

        /// <summary>
        /// Serialize a strongly typed configuration object to a JSON formatted
        /// string.
        /// </summary>
        /// <param name="data">A strongly typed configuration object.</param>
        /// <returns>The JSON formatted string.</returns>
        public string Serialize(object data)
        {
            if (data == null)
                throw new ArgumentNullException();

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            };

            return JsonConvert.SerializeObject(data, settings);
        }
    }
}
