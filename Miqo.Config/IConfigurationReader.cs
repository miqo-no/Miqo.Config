using System.IO;

namespace Miqo.Config {
    /// <summary>
    /// Represents a fluent interface to load a strongly typed configuration
    /// object from a configuration store.
    /// </summary>
    public interface ICanSelectConfigurationStoreToLoad {
        /// <summary>
        /// Application wide configurations are saved in the same directory as
        /// the application itself.
        /// </summary>
        /// <returns></returns>
        ICanLoadConfiguration ApplicationSettings();

        /// <summary>
        /// Application wide configurations are saved in the specified
        /// directory.
        /// </summary>
        /// <param name="directory">The directory to use.</param>
        /// <returns></returns>
        ICanLoadConfiguration ApplicationSettings(string directory);

        /// <summary>
        /// User specific configurations are saved in the currently logged in
        /// user's roaming application data folder.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the application can be used as a subfolder in the
        /// roaming application data folder.
        /// </param>
        /// <returns></returns>
        ICanLoadConfiguration UserSettings(string applicationName);
    }

	public interface ICanLoadConfiguration
	{
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
        T FromFile<T>(string file, bool useAbsolutePath = false) where T : new();

        /// <summary>
        /// Deserializes a <see cref="Stream"/> to a strongly typed
        /// configuration object.
        /// </summary>
        /// <typeparam name="T">The strongly typed configuration object.</typeparam>
        /// <param name="stream">The <see cref="Stream"/> to deserialize.</param>
        /// <returns>
        /// A strongly typed configuration object.
        /// </returns>
        T FromStream<T>(Stream stream) where T : new();

        /// <summary>
        /// Deserializes a <see cref="string"/> to a strongly typed
        /// configuration object.
        /// </summary>
        /// <typeparam name="T">The strongly typed configuration object.</typeparam>
        /// <param name="data">The <see cref="string"/> to deserialize.</param>
        /// <returns>A strongly typed configuration object.</returns>
	    T FromString<T>(string data) where T : new();
    }
}
