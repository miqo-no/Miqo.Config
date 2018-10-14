namespace Miqo.Config.Formatters
{
    /// <summary>
    /// Represents a configuration formatter.
    /// </summary>
    public interface IConfigurationFormatter
    {
        /// <summary>
        /// Parse a string to create a strongly typed configuration object.
        /// </summary>
        /// <typeparam name="T">Strongly typed configuration object.</typeparam>
        /// <param name="data">The string to parse.</param>
        /// <returns>A strongly typed configuration object.</returns>
        T Parse<T>(string data) where T : new();

        /// <summary>
        /// Serialize a strongly typed configuration object to a string.
        /// </summary>
        /// <param name="data">A strongly typed configuration object.</param>
        /// <returns>The serialized string.</returns>
        string Serialize(object data);
    }
}
