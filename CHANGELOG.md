# Change Log

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/)
and this project adheres to [Semantic Versioning](http://semver.org/).

<!-- Available types of changes:
### Added
### Changed
### Fixed
### Deprecated
### Removed
### Security
-->

## [Unreleased]

### Added

- Connect when running in Flatpak. App needs to set USER_CONFIG_HOME to host
  XDG_CONFIG_HOME. Flatpak is detected from the presence of FLATPAK_ID.

## [2.0.3] - 2020-08-28

### Fixed

- Remove dependency on `SIL.ReleaseTasks`

## [2.0.2] - 2020-08-28

### Fixed

- Ignore comments in IBus config file. This fixes things on Ubuntu 20.04.

## [2.0.1] - 2019-10-22

### Changed

- Update to use .NET Framework 4.6.1
- Sign assembly
