# Miqo.Config, the easy to use configuration file manager

Writing repetitive code to manage, read and write configuration files for every project is tedious. Let Miqo.Config take care of the heavy lifting of managing configuration files for you, so you can focus on your project.

Miqo.Config helps translate your strongly typed object to a JSON configuration file.

## Adding Miqo.Config to Your Project

The library is available as a NuGet package.

```
PM> Install-Package Miqo.Config
```

## Creating the Configuration Class

Start by creating a class to hold your configurations.

```csharp
public class Configuration {
	public string Server { get; set; }
	public int Port { get; set; }
	public List<string> { get; set; }
}
```

## Reading a Configuration File

Reading the application settings from a JSON file is done in the following way:

```csharp
var config = new Miqo.Config.ConfigurationManager()
	.ApplicationSettings()
	.LoadConfigurationFromFile<Configuration>("Spiffy.json");

Console.Writeline(config.ConnectionString);
```

Miqo.Config can also load a configuration from a JSON based string:

```csharp
var string json = "{ \"Server\": \"localhost\" }";

var config = new Miqo.Config.ConfigurationManager()
	.ApplicationSettings()
	.LoadConfigurationFromString<Configuration>(json);

Console.Writeline(config.ConnectionString);
```

Application wide configurations are stored in the same directory as the application. A custom location can be specified using ```ApplicationSettings(string directory)```.

## Writing Settings to a Configuration File

```csharp
var config = new Configuration {
	Server = "localhost",
	Port = 8080,
	IndexFiles = new List<string> {"index.html", "index.htm", "index.php"}
};

new Miqo.Config.ConfigurationManager()
	.ApplicationSettings()
	.SaveConfiguration(config)
	.ToFile("Spiffy.json");
```
The following file will be created in the application's folder:

```json
{
	"Server": "localhost",
	"Port": 8080,
	"IndexFiles": [
		"index.html",
		"index.htm",
		"index.php"
	]
}
```

## User Specific Settings

You can have application and user specific settings that are unrelated to each other. For instance, you can save the main window's position and size in it's own configuration file.

```csharp
new Miqo.Config.ConfigurationManager()
	.UserSettings("SpiffyApp")
	.SaveConfiguration(config)
	.ToFile("Spiffy.json");
```

Use ```UserSettings(string appName)``` instead of ```ApplicationSettings()``` to save the configuration to the currently logged in user's ApplicationData folder. You can specify a subfolder for your particular application's data.

## Additional features

Miqo.Config has some other nifty features that may be useful to you as a developer.

### Logging

You can add logging capabilities to Miqo.Config by using the Log delegate. You add Serilog logging in the following way:

```csharp
var log = Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.CreateLogger();

var cm = new Miqo.Config.ConfigurationManager {
	Log = log.Information,
	LogException = log.Error
};

cm.ApplicationSettings()
	.SaveConfiguration(config)
	.ToFile("config.json");
```

### Protecting Sensitive Information

If you are storing usernames, password, connection strings, API keys or any other such sensitive data, you should consider encrypting the property. Add the ```[JsonConverter(typeof(EncryptedPropertyConverter), key)]``` attribute to the property.

Example:

```csharp
[JsonConverter(typeof(EncryptedPropertyConverter), "8ef51d43-03b9-4831-b415-5c73d472340d")]
public string ConnectionString { get; set; }
```

Miqo.Config will encrypt the information before writing the property to the configuration file, and decrypt the information back into the property upon reading the configuration file. You can set your own key on a project to project basis. In the example above, a random GUID has been used as a pass phrase. Miqo.Config uses AES Rijndael encryption.


### Ignoring Properties

If you would like to prevent properties from being serialized to the configuration file, use the ```[JsonIgnore]``` attribute.

```csharp
[JsonIgnore]
public string NotReallyAllThatImportant { get; set; }
```
