<h1 align="center">
   <br>
   <img src ="./.github/miqo.config.png" width="128" height="128"/>
   <br>
   Miqo.Config
   <br>
</h1>
<h3 align="center">
   :page_facing_up::star2: Managing application and user settings for your .NET application has never been so easy
</h3>
<p align="center">
<a href="https://ci.appveyor.com/project/natsuo/miqo-config"><img src="https://img.shields.io/appveyor/ci/natsuo/miqo-config.svg?style=for-the-badge&logo=appveyor"/></a>
<a href="https://travis-ci.org/miqo-no/Miqo.Config"><img src="https://img.shields.io/travis/miqo-no/Miqo.Config.svg?style=for-the-badge&logo=travis"></a>
<a href="./LICENSE.md"><img src=".github/mit.svg"/></a>
</p>

## Overview

Writing repetitive code to manage, read and write configuration files for every project is tedious. Let Miqo.Config take care of the heavy lifting of managing configuration files for you, so you can focus on your project.

Miqo.Config is a .NET Standard 2.0 library that helps translate your strongly typed object to a JSON configuration file.

## Adding Miqo.Config to Your Project

The library is available as a signed NuGet package.

```
PM> Install-Package Miqo.Config
```

## Creating the Configuration Class

Start by creating a class to hold your configurations.

```csharp
public class Configuration {
   public string Server { get; set; }
   public int Port { get; set; }
   public List<string> IndexFiles { get; set; }
}
```

## Reading a Configuration File

Reading the application settings from a JSON file is done in the following way:

```csharp
using Miqo.Config;

var config = new MiqoConfig()
   .Load()
   .ApplicationSettings()
   .FromFile<Configuration>("Spiffy.json");

Console.Writeline(config.Server);
```

Miqo.Config can also load a configuration from a JSON based string:

```csharp
using Miqo.Config;

var string json = "{ \"Server\": \"localhost\" }";

var config = new MiqoConfig()
   .Load()
   .ApplicationSettings()
   .FromString<Configuration>(json);

Console.Writeline(config.Server);
```

Application wide configurations are stored in the same directory as the application. A custom location can be specified using ```ApplicationSettings(string directory)```.

## Writing Settings to a Configuration File

```csharp
using Miqo.Config;

var config = new Configuration {
   Server = "localhost",
   Port = 8080,
   IndexFiles = new List<string> {"index.html", "index.htm", "index.php"}
};

new MiqoConfig()
   .Save(config)
   .ApplicationSettings()
   .ToFile("Spiffy.json");
```

The following file will be created in the application's folder:

```json
{
   "server": "localhost",
   "port": 8080,
   "indexFiles": [
      "index.html",
      "index.htm",
      "index.php"
   ]
}
```

## User Specific Settings

You can have application and user specific settings that are unrelated to each other. For instance, you can save the main window's position and size in it's own configuration file.

```csharp
new MiqoConfig()
   .Save(config)
   .UserSettings("SpiffyApp")
   .ToFile("Spiffy.json");
```

Use ```UserSettings(string appName)``` instead of ```ApplicationSettings()``` to save the configuration to the currently logged in user's ApplicationData folder. You can specify a subfolder for your particular application's data.

## Additional features

Miqo.Config has some other nifty features that may be useful to you as a developer.

### Logging

You can add logging capabilities to Miqo.Config. Serilog can be added as such:

```csharp
var logger = Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
   .CreateLogger();

var json = new MiqoConfig(logger)
    .Save(config)
    .ApplicationSettings()
    .ToString();
```

### Protecting Sensitive Information

If you are storing usernames, password, connection strings, API keys or any other such sensitive data, you should consider encrypting the property. Add the ```[JsonConverter(typeof(EncryptedPropertyConverter), key)]``` attribute to the property.

Example:

```csharp
[JsonConverter(typeof(EncryptedPropertyConverter), "cfVMjtOJ8/eJx0037MHNym3awHj9iAUBdM/bmiLUvlc=")]
public string ConnectionString { get; set; }
```

Miqo.Config will encrypt the information before writing the property to the configuration file, and decrypt the information back into the property upon reading the configuration file. You can set your own key on a project to project basis. Miqo.Config uses AES to encrypt sensitive information.

Use the helper method `StringCipher.CreateRandomKey()` to create a random AES key.

### Ignoring Properties

If you would like to prevent properties from being serialized to the configuration file, use the ```[JsonIgnore]``` attribute.

```csharp
[JsonIgnore]
public string NotReallyAllThatImportant { get; set; }
```

## Acknowledgements

The encryption code is based on code by [Ckode.Encryption](https://github.com/NQbbe/Ckode.Encryption/) by Steffen Skov.

## License

Miqo.License is made available under the [MIT License](LICENSE).
