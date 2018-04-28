# Zyan.SafeDeserializationHelpers

This tiny library tries to fix several known BinaryFormatter vulnerabilities.
When a malicious payload is detected, the library throws an `UnsafeDeserializationException`
instead of deserializing the data that is able to produce bad side effects. 

[![appveyor](https://ci.appveyor.com/api/projects/status/github/zyanfx/safedeserializationhelpers?svg=true)](https://ci.appveyor.com/project/yallie/safedeserializationhelpers)
[![tests](https://img.shields.io/appveyor/tests/yallie/safedeserializationhelpers.svg)](https://ci.appveyor.com/project/yallie/safedeserializationhelpers/build/tests)
[![nuget](https://img.shields.io/nuget/v/Zyan.SafeDeserializationHelpers.svg)](https://nuget.org/packages/Zyan.SafeDeserializationHelpers)

# Deserializing the untrusted data is dangerous

It's proven that deserialing arbitrary payloads under certain conditions can
trigger code execution. BinaryFormatter, DataContractSerializer, XmlSerializer,
as well as several widely used JSON serializers are known to be vulnerable.

See [ysoserial.net](https://github.com/pwntester/ysoserial.net) project for details.

# Code sample

```csharp
// unsafe: deserialization can trigger arbitrary code execution
var fmt = new BinaryFormatter();
var object = fmt.Deserialize(stream);

// safe: deserialization is guarded against known vulnerabilities
var fmt = new BinaryFormatter().Safe();
var object = fmt.Deserialize(stream);
```

# Usage

1. Install [Zyan.SafeDeserializationHelpers](https://www.nuget.org/packages/Zyan.SafeDeserializationHelpers) nuget package.
2. Use `new BinaryFormatter().Safe()` instead of just `new BinaryFormatter()`.
3. For .NET Remoting projects, use safe versions of the binary formatter sinks:
   * Replace `BinaryClientFormatterSinkProvider` with `SafeBinaryClientFormatterSinkProvider`.
   * Replace `BinaryServerFormatterSinkProvider` with `SafeBinaryServerFormatterSinkProvider`.
4. Make sure to test your project against payloads produced by [ysoserial.net](https://github.com/pwntester/ysoserial.net) gadgets.

# Known vulnerabilities supported by the library

* **ActivitySurrogateSelector** gadget by James Forshaw (loads an assembly and executes arbitrary code).
* **PSObject** gadget by Oleksandr Mirosh and Alvaro Munoz. Target must run a system not patched for CVE-2017-8565.
* **TypeConfuseDelegate** gadget by James Forshaw (runs any process using Process.Start delegate).
* **DataSet** gadget by James Forshaw (unsafe BinaryFormatter deserialization).
* **WindowsIdentity** gadget by Levi Broderick (unsafe BinaryFormatter deserialization).

# References

* [Exploiting .NET Managed DCOM](https://googleprojectzero.blogspot.com/2017/04/exploiting-net-managed-dcom.html) by James Forshaw
* [Are you my Type?](https://media.blackhat.com/bh-us-12/Briefings/Forshaw/BH_US_12_Forshaw_Are_You_My_Type_WP.pdf) by James Forshaw
* [Attacking .NET serialization](https://speakerdeck.com/pwntester/attacking-net-serialization) by Alvaro Munoz
* [Friday the 13th: JSON Attacks](https://www.blackhat.com/docs/us-17/thursday/us-17-Munoz-Friday-The-13th-Json-Attacks.pdf) by Alvaro Munoz and Oleksandr Mirosh
* [Severe Deserialization Issues Also Affect .NET, Not Just Java](https://www.bleepingcomputer.com/news/security/severe-deserialization-issues-also-affect-net-not-just-java/) by Catalin Cimpanu
* [ysoserial.net](https://github.com/pwntester/ysoserial.net) exploit collection by Alvaro Munoz

# Thanks

* [Markus Wulftange](https://github.com/mwulftange) — for bringing up the problem to my attention
* [James Forshaw](https://github.com/tyranid) — for the great blog posts, papers and talks on the subject
* [Alvaro Munoz](https://github.com/pwntester), Oleksandr Mirosh — for the awesome educational ysoserial.net project
* [Chris Frohoff](https://github.com/frohoff) — for the original ysoserial Java project
* [Levi Broderick](https://github.com/GrabYourPitchforks) — for more malicious gadgets

# License

MIT License.
