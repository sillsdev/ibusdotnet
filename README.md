# ibusdotnet

ibusdotnet is a managed wrapper around IBus.

## Installation

Get the [ibusdotnet](https://www.nuget.org/packages/ibusdotnet/) nuget package.

## Building from source

The project can be built with the following command:

```bash
msbuild build/ibusdotnet.proj -t:Build
```

Run tests:

```bash
msbuild build/ibusdotnet.proj -t:Test
```

Compile test app:

```bash
msbuild ibusTextBoxTest
```