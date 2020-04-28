# EventSourceProxy #

**EventSourceProxy** (ESP) is the easiest way to add scalable Event Tracing for Windows (ETW) logging to your .NET program.

## Important ##

This repository is a fork of the Jon Wagner's original source code. It was created primarily to allow this library to
be consumed by applications targeting .NET Standard 2.1 _or_ .NET Framework 4.7.2.

 ### Limitations

 To allow this library to be more easily maintained we have dropped support for the manifest generation tool and the NuGet
 tracing library.

 Note that proxy generation for interfaces with generic parameters is broken and we have no plans to fix this.
 Feel free to submit a pull request however.

### Compatibility

The library _may_ be backwards compatible with earlier versions of the .NET Framework however this is not an aim or priority
for the maintainers of this repository. We would recommend that if you do not need to target .NET Core that you use the
original version of the library in the source repository.

The library will not work with earlier versions of .NET Standard because the reflection APIs used to generate proxy
logging classes are not available in those versions.

 ### Issues

 Please feel free to open an issue in GitHub if you find bugs in the current version of the library.

 However, bear in mind that we maintain this package mainly for our own consumption so we may choose not to fix the
 issue ourselves if it is not in our direct interests.

 We will accept PRs to add new features or fix existing issues if they are reasonable and accompanied by relevant tests.

### NuGet

The version of the library in this repository is published as `ECargo.EventSourceProxy.NetStandard`.

To add it to your .NET Core project use:
```
dotnet add package ECargo.EventSourceProxy.NetStandard
```

## Why You Want This ##

- You really should be logging more than you do now.
- ETW is the best way to log in Windows.
- It's about zero effort to add logging to new code.
- It's zero effort to add logging to existing interfaces.
- Generated IL keeps overhead low, and it's almost nothing if tracing is off.

Here is ESP implementing a logging interface for you automatically:

```csharp
public interface ILog
{
    void SomethingIsStarting(string message);
    void SomethingIsFinishing(string message);
}

// yeah, this is it
var log = EventSourceImplementer.GetEventSourceAs<ILog>();

log.SomethingIsStarting("hello");
log.SomethingIsFinishing("goodbye");
```

Here is ESP doing the hard work of implementing an EventSource if you really want to do that:

```csharp
public abstract MyEventSource : EventSource
{
    public abstract void SomethingIsStarting(string message);
    public abstract void SomethingIsFinishing(string message);
}

// ESP does the rest
var log = EventSourceImplementer.GetEventSourceAs<MyEventSource>();
```

Here is ESP wrapping an existing interface for tracing:

```csharp
public interface ICalculator
{
    void Clear();
    int Add(int x, int y);
    int Multiple(int x, int y);
}

public class Calculator : ICalculator
{
    // blah blah
}

Calculator calculator = new Calculator();

// again, that's it
ICalculator proxy = TracingProxy.CreateWithActivityScope<ICalculator>(calculator);

// all calls are automatically logged when the ETW source is enabled
int total = proxy.Add(1, 2);
```

And let's say that your interface doesn't look at all like what you want logged. You can add rules to clean all that up:

Say you have the following interface:

```csharp
interface IEmailer
{
    void Send(Email email, DateTime when);
    void Receive(Email email);
    void Cancel(string from, string to, DateTime earliest, DateTime latest);
}

class Email
{
    public string From;
    public string To;
    public string Subject;
    public string Body;
    public IEnumerable<byte[]> Attachments;
}
```

Set up rules on how ESP should trace the data to ETW:

```csharp
    TraceParameterProvider.Default
        .For<IEmailer>()
            .With<Email>()
                .Trace(e => e.From).As("Sender")
                .Trace(e => e.To).As("Recipient")
                .Trace(e => e.Subject).As("s")
                     .And(e => e.Body).As("b")
                    .TogetherAs("message")
                .Trace(e => String.Join("/", e.Attachments.Select(Convert.ToBase64String).ToArray()))
                    .As("attachments")
        .For<IEmailer>(m => m.Send(Any<Email>.Value, Any<DateTime>.Value)
            .Ignore("when");
        .ForAnything()
            .AddContext("user", () => SomeMethodThatChecksIdentity());
```

And now the Send method will log:

    * Sender : From
    * Recipient : To
    * Message : { "s":subject, "b":body }
    * Attachments : [ base64, base64 ]
    * User : current user

So, this is great for adding logging to any interface in your application.

# Features #

* Automatically implement logging for any interface, class derived from EventSource.
* Takes all of the boilerplate code out of implementing an EventSource.
* Supports EventSource and Event attributes for controlling the generated events.
* Supports reusing Keyword, Opcode, and Task enums in multiple log sources.
* Automatically proxy any interface and create a logging source.
* Add rules to transform parameters and objects from your interface to your log.
* Proxies also implement _Completed and _Faulted events.
* Automatically convert complex types to JSON strings for logging.
* Optionally override the object serializer.
* Optionally provide a logging context across an entire logging interface.
* Easily manage Activity IDs with EventActivityScope.

# New in v2.0 - Logging Transforms #

* Use attributes and configuration rules to transform your interface calls to entirely different logging calls. See [Controlling Logged Data](https://github.com/jonwagner/EventSourceProxy/wiki/Controlling-Logged-Data) and [Adding Additional Logging Context](https://github.com/jonwagner/EventSourceProxy/wiki/Adding-Additional-Logging-Context).

# Documentation #

**Full documentation is available on the [wiki](https://github.com/jonwagner/EventSourceProxy/wiki)!**

# Good References #

* Want to get the data out of ETW? Use the [Microsoft Enterprise Library Semantic Logging Application Block](http://nuget.org/packages/EnterpriseLibrary.SemanticLogging/). It has ETW listeners to log to the console, rolling flat file, and databases, so you can integrate ETW with your existing log destinations.
