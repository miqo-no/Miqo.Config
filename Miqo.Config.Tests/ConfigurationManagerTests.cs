using System;
using Newtonsoft.Json;
using Xunit;

namespace Miqo.Config.Tests {
	public class ConfigurationManagerTests {
		[Fact]
		public void CanReadAnExampleConfiguration() {
			const string json = "{ \"ConnectionString\": \"localhost\" }";

			var cm = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.Configuration>(json);

			Assert.Equal("localhost", cm.ConnectionString);
		}

		[Fact]
		public void HandlesNonExistingConfigurationFilesGracefully() {
			var cm = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromFile<ConfigClasses.Configuration>("a_file_that_doesnt_exist.json");

			Assert.Null(cm.ConnectionString);
		}

		[Fact]
		public void SerializedConfigurationCanBeDeserialized() {
			var config = new ConfigClasses.ConfigurationWithEncryption {
				ServerName = "127.0.0.0"
			};

			var s = new ConfigurationManager()
				.ApplicationSettings()
				.SaveConfiguration(config)
				.ToString();

			var deserializedConfig = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.ConfigurationWithEncryption>(s);

			Assert.Equal(config.ServerName, deserializedConfig.ServerName);
		}

		[Fact]
		public void PropertiesCanBeEncryptedAndDecrypted() {
			var config = new ConfigClasses.ConfigurationWithEncryption {
				ConnectionString = "super_secret_connection_string"
			};

			var s = new ConfigurationManager()
				.ApplicationSettings()
				.SaveConfiguration(config)
				.ToString();

			Assert.DoesNotContain("super_secret_connection_string", s);

			var deserializedConfig = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.ConfigurationWithEncryption>(s);

			Assert.Equal("super_secret_connection_string", deserializedConfig.ConnectionString);
		}

		[Fact]
		public void DeserializeEncryptedPropertyWithIncorrectKey() {
			const string json = "{ \"ConnectionString\": \"nR5/7PndFiYql9uGH2G6ZO7zsl7l1SoUzKNLVuYjmvAFRNzc+b22sM65awJCb75mbMMGR0pEXf9qJH83hi6Q3fkSkp5uQZf4QIiIe4/rAS7vaSqM40qKM8IrEXgZjpj3\", \"ServerName\": \"localhost\", \"PortNumber\": 80 }";

			var config = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.ConfigurationWithIncorrectEncryption>(json);

			Assert.Null(config.SnafuString);
		}

		[Fact]
		public void DeserializeEncryptedPropertyWithIncorrectCipher() {
			const string json = "{ \"ConnectionString\": \"this_string_isnt_encrypted\", \"ServerName\": \"localhost\", \"PortNumber\": 80 }";

			var config = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.ConfigurationWithIncorrectEncryption>(json);

			Assert.Null(config.SnafuString);
		}

		[Fact]
		public void DeserializedPropertiesRetainsDefaultValues() {
			const string json = "{ \"ConnectionString\": null }";
			var config = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.ConfigurationWithEncryption>(json);

			Assert.Null(config.ConnectionString);
			Assert.Equal("localhost", config.ServerName);
			Assert.Equal(80, config.PortNumber);
		}

		[Fact]
		public void DeserializedPropertiesArentOverwrittenWithDefaults() {
			const string json = "{ \"PortNumber\": 8080 }";
			var config = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.ConfigurationWithEncryption>(json);

			Assert.Equal(8080, config.PortNumber);
		}

		[Fact]
		public void DeserializePropertiesWithIncorrectTypeUsingDefaultValue()
		{
			const string json = "{ \"PortNumber\": \"funky-value\" }";

			var config = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.ConfigurationWithEncryption>(json);

			// Uses default value provided in class, or type's default
			Assert.Equal(80, config.PortNumber);
		}

		[Fact]
		public void PropertiesWithIgnoreAttributeArentSerialized() {
			var config = new ConfigClasses.ConfigurationWithEncryption {
				IgnoredVariable = "super_secret_variable"
			};

			var s = new ConfigurationManager()
				.ApplicationSettings()
				.SaveConfiguration(config)
				.ToString();

			var deserializedConfig = new ConfigurationManager()
				.ApplicationSettings()
				.LoadConfigurationFromString<ConfigClasses.ConfigurationWithEncryption>(s);

			Assert.Null(deserializedConfig.IgnoredVariable);
		}

		// TODO: Incorrect key
		// TODO: Unreadable encrypted text
		// TODO: Incorrect data
	}
}