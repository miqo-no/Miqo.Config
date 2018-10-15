using System;
using System.Collections.Generic;
using System.Text;
using Miqo.Config.Tests.ConfigClasses;
using Newtonsoft.Json;
using Xunit;

namespace Miqo.Config.Tests
{
    public class MiqoConfigTests
    {
        [Fact]
        public void Load_CanReadValidString()
        {
            const string json = "{ \"ConnectionString\": \"localhost\" }";

            var config = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationDb>(json);
            
            Assert.Equal("localhost", config.ConnectionString);
        }

        [Fact]
        public void ToString_CanSerializeValidJson_WhenValidConfiguration()
        {
            var expected = new ConfigClasses.Configuration
            {
                Title = "Children of the Gods",
                Season = 1,
                Episode = 1,
                AiredOn = new DateTime(1997, 7, 27)
            };

            var json = new MiqoConfig()
                .Save(expected)
                .ApplicationSettings()
                .ToString();

            Assert.NotEmpty(json);

            var actual = JsonConvert.DeserializeObject<ConfigClasses.Configuration>(json);
            Assert.Equal(expected.Title, actual.Title);
            Assert.Equal(expected.Season, actual.Season);
            Assert.Equal(expected.Episode, actual.Episode);
            Assert.Equal(expected.AiredOn, actual.AiredOn);
        }

        [Fact]
        public void ToString_DropsPropertyFromJson_WhenNullValue()
        {
            var expectedJson =
                "{\r\n  \"title\": \"The Enemy Within\",\r\n  \"season\": 1,\r\n  \"episode\": 3,\r\n  \"airedOn\": null\r\n}";

            var expected = new ConfigClasses.Configuration
            {
                Title = "The Enemy Within",
                Season = 1,
                Episode = 3
            };

            var json = new MiqoConfig()
                .Save(expected)
                .ApplicationSettings()
                .ToString();

            Assert.NotEmpty(json);

            var actual = JsonConvert.DeserializeObject<ConfigClasses.Configuration>(json);
            Assert.Null(actual.AiredOn);
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void CanReadAnExampleConfiguration()
        {
            const string json = "{ \"ConnectionString\": \"localhost\" }";

            var cm = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationDb>(json);

            Assert.Equal("localhost", cm.ConnectionString);
        }

        [Fact]
        public void HandlesNonExistingConfigurationFilesGracefully()
        {
            var cm = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromFile<ConfigClasses.ConfigurationDb>("a_file_that_doesnt_exist.json");

            Assert.Null(cm.ConnectionString);
        }

        [Fact]
        public void SerializedConfigurationCanBeDeserialized()
        {
            var config = new ConfigClasses.ConfigurationWithEncryption
            {
                ServerName = "127.0.0.0"
            };

            var s = new MiqoConfig()
                .Save(config)
                .ApplicationSettings()
                .ToString();

            var deserializedConfig = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationWithEncryption>(s);

            Assert.Equal(config.ServerName, deserializedConfig.ServerName);
        }

        [Fact]
        public void PropertiesCanBeEncryptedAndDecrypted()
        {
            var config = new ConfigClasses.ConfigurationWithEncryption
            {
                ConnectionString = "super_secret_connection_string"
            };

            var s = new MiqoConfig()
                .Save(config)
                .ApplicationSettings()
                .ToString();

            Assert.DoesNotContain("super_secret_connection_string", s);

            var deserializedConfig = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationWithEncryption>(s);

            Assert.Equal("super_secret_connection_string", deserializedConfig.ConnectionString);
        }

        [Fact]
        public void DeserializeEncryptedPropertyWithIncorrectKey()
        {
            const string json =
                "{ \"ConnectionString\": \"nR5/7PndFiYql9uGH2G6ZO7zsl7l1SoUzKNLVuYjmvAFRNzc+b22sM65awJCb75mbMMGR0pEXf9qJH83hi6Q3fkSkp5uQZf4QIiIe4/rAS7vaSqM40qKM8IrEXgZjpj3\", \"ServerName\": \"localhost\", \"PortNumber\": 80 }";

            var config = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationWithIncorrectEncryption>(json);

            Assert.Null(config.SnafuString);
        }

        [Fact]
        public void DeserializeEncryptedPropertyWithIncorrectCipher()
        {
            const string json =
                "{ \"ConnectionString\": \"this_string_isnt_encrypted\", \"ServerName\": \"localhost\", \"PortNumber\": 80 }";

            var config = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationWithIncorrectEncryption>(json);

            Assert.Null(config.SnafuString);
        }

        [Fact]
        public void DeserializedPropertiesRetainsDefaultValues()
        {
            const string json = "{ \"ConnectionString\": null }";
            var config = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationWithEncryption>(json);

            Assert.Null(config.ConnectionString);
            Assert.Equal("localhost", config.ServerName);
            Assert.Equal(80, config.PortNumber);
        }

        [Fact]
        public void DeserializedPropertiesArentOverwrittenWithDefaults()
        {
            const string json = "{ \"PortNumber\": 8080 }";
            var config = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationWithEncryption>(json);

            Assert.Equal(8080, config.PortNumber);
        }

        [Fact]
        public void DeserializePropertiesWithIncorrectTypeUsingDefaultValue()
        {
            const string json = "{ \"PortNumber\": \"funky-value\" }";

            var config = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationWithEncryption>(json);

            // Uses default value provided in class, or type's default
            Assert.Equal(80, config.PortNumber);
        }

        [Fact]
        public void PropertiesWithIgnoreAttributeArentSerialized()
        {
            var config = new ConfigClasses.ConfigurationWithEncryption
            {
                IgnoredVariable = "super_secret_variable"
            };

            var s = new MiqoConfig()
                .Save(config)
                .ApplicationSettings()
                .ToString();

            var deserializedConfig = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromString<ConfigClasses.ConfigurationWithEncryption>(s);

            Assert.Null(deserializedConfig.IgnoredVariable);
        }

        [Fact]
        public void SerializeAndDeserializeUsingCustomFormatter()
        {
            var formatter = new XmlConfigurationFormatter(typeof(ConfigurationDb));
            var expected = new ConfigurationDb
            {
                ConnectionString = "the_connection_string"
            };

            var xml = new MiqoConfig(formatter)
                .Save(expected)
                .ApplicationSettings()
                .ToString();

            Assert.NotEmpty(xml);

            var actual = new MiqoConfig(formatter)
                .Load()
                .ApplicationSettings()
                .FromString<ConfigurationDb>(xml);

            Assert.Equal(expected.ConnectionString, actual.ConnectionString);
        }

        [Fact]
        public void CanReadConfigurationStream()
        {
            var expected = new ConfigurationDb
            {
                ConnectionString = "the_connection_string"
            };

            var stream = new MiqoConfig()
                .Save(expected)
                .ApplicationSettings()
                .ToStream();

            stream.Position = 0;

            var actual = new MiqoConfig()
                .Load()
                .ApplicationSettings()
                .FromStream<ConfigurationDb>(stream);

            Assert.Equal(expected.ConnectionString, actual.ConnectionString);
        }
    }
}
