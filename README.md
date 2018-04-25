# Deserializing the untrusted data is dangerous

This tiny library tries to fix several known BinaryFormatter vulnerabilities.  
See [ysoserial.net](https://github.com/pwntester/ysoserial.net) project for details.

[![appveyor](https://ci.appveyor.com/api/projects/status/github/zyanfx/safedeserializationhelpers?svg=true)](https://ci.appveyor.com/project/yallie/safedeserializationhelpers)
[![tests](https://img.shields.io/appveyor/tests/yallie/safedeserializationhelpers.svg)](https://ci.appveyor.com/project/yallie/safedeserializationhelpers/build/tests)

# Code sample

```csharp
// bad: deserialization can trigger arbitrary code execution
var fmt = new BinaryFormatter();
var object = fmt.Deserialize(stream);

// better: deserialization is checked against known vulnerabilities
var fmt = new BinaryFormatter().Safe();
var object = fmt.Deserialize(stream);
```

# Usage

TODO: publish a Nuget package