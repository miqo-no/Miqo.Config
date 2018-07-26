# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/). Note that AppVeyor build number and the release version numbers may not be in sync.

## Unreleased

### Implemented enhancements

* Add CHANGELOG.md file to project [#6](https://github.com/miqo-no/Miqo.Config/issues/6)

### Fixed bugs

* Fix README.md with regards to supported versions of .NET  [Commit: 574f3d5](https://github.com/miqo-no/Miqo.Config/commit/574f3d5189f1c8d8a9b7873708bc2bfc8a52d288)
* Fix NuGet package information with regards to supported versions of .NET [Commit: 9ff0179](https://github.com/miqo-no/Miqo.Config/commit/9ff01797f4c96d7856af7c71b26d502dd6b61b77)

## [v1.1.0](https://github.com/miqo-no/Miqo.Config/releases/tag/v1.1.0) (2017-07-26)

### Implemented enhancements

* New README.md file
* Restructure project using new .csproj format
* Add [Key Creation Tool](https://github.com/miqo-no/Miqo.Config/blob/master/Miqo.Config.CreateKeys) to the project to help developers create a new private key for their product

### Breaking changes

* **BREAKING**: Miqo.Config now runs on .NET Standard 2.0, .NET Core 2.0 and .NET 4.6.1
* **BREAKING**: Encryption code has been changed. [Commit 9ff0179](https://github.com/miqo-no/Miqo.Config/commit/574f3d5189f1c8d8a9b7873708bc2bfc8a52d288)<br>
The code that handles sensitive information encryption and decryption has been changed. Miqo.Config now uses the [Ckode.Encryption](https://github.com/NQbbe/Ckode.Encryption/) library by Steffen Skov for AES encryption and decryption.<br><br>
**Private keys in version 1.0 is not compatible with v1.1. Files that contain encrypted settings from v1.0 will not be decrypted by v1.1.**

I apologize for the extra work and frustration this entails for you as a developer. Any such changes in the future will result in a deprecation notice and the ability to transfer settings.
