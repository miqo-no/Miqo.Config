using System.IO;

namespace Miqo.Config
{
    /// <summary>
    /// Represents a fluent interface to save a strongly typed configuration
    /// object to a configuration store.
    /// </summary>
    public interface ICanSelectConfigurationStoreToSave
    {
        /// <summary>
        /// Application wide configurations are saved in the same directory as
        /// the application itself.
        /// </summary>
        /// <returns></returns>
        ICanSaveConfiguration ApplicationSettings();

        /// <summary>
        /// Application wide configurations are saved in the specified
        /// directory.
        /// </summary>
        /// <param name="directory">The directory to use.</param>
        /// <returns></returns>
        ICanSaveConfiguration ApplicationSettings(string directory);

        /// <summary>
        /// User specific configurations are saved in the currently logged in
        /// user's roaming application data folder.
        /// </summary>
        /// <param name="applicationName">
        /// The name of the application can be used as a subfolder in the
        /// roaming application data folder.
        /// </param>
        /// <returns></returns>
        ICanSaveConfiguration UserSettings(string applicationName);
    }

    public interface ICanSaveConfiguration
    {
        /// <summary>
        /// Serializes the configuration object to a file.
        /// </summary>
        /// <param name="file">
        /// The name of the configuration file including extension name.
        /// </param>
        /// <param name="useAbsolutePath">
        /// Specifies if the <paramref name="file"/> is an absolute path.
        /// </param>
        void ToFile(string file, bool useAbsolutePath = false);

        /// <summary>
        /// Serializes the configuration to a <see cref="Stream"/>.
        /// </summary>
        /// <returns>The <see cref="Stream"/>.</returns>
        Stream ToStream();

        /// <summary>
        /// Serializes the strongly typed configuration object to a string.
        /// </summary>
        /// <returns>The serialized string.</returns>
        string ToString();
    }
}
