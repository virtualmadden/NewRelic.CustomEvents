# NewRelic.CustomEvents
.NET Standard library supporting custom event reporting to the NewRelic Insights product.

|Master|
|:--:|
|[![Build Status](https://travis-ci.org/virtualmadden/NewRelic.CustomEvents.svg?branch=master)](https://travis-ci.org/virtualmadden/NewRelic.CustomEvents)|

## Attributes

### Event Attributes
`[NewRelicIgnore]`</br>
Decorate class properties with this attribute if you would like them to not be included when sending a custom event to NewRelic. Restricted to class properties.

`[NewRelicName("FooBar")]`</br>
Decorate classes and class properties with this attribute if you would like to rename items when sending a custom event to NewRelic. Restricted to classes and class properties.

## Usage
```C#
public static void LogCustomEvent(string serviceName)
{
    new ServiceEvent(serviceName)
    {
        OperationName = "CustomEvent",
        OperationLevel = OperationLevel.System,
        OperationSuccess = true
    }.SendAsync();
}
```

```C#
public static void LogCustomEvent(string serviceName)
{
    new ServiceEvent(serviceName)
    {
        OperationName = "CustomEvent",
        OperationLevel = OperationLevel.System,
        OperationSuccess = false,
        ErrorMessage = "Something went wrong!"
    }.SendAsync();
}
```

```C#
[NewRelicName("ExtraCustomEvent")]                // Sent as an ExtraCustomEvent.
public class CustomServiceEvent : BaseEvent
{
    public CustomServiceEvent(string serviceName, Guid? requestId = null) 
    : base(serviceName, requestId) { }

    [NewRelicIgnore]
    public string Username { get; set; }    // Not sent with event.
    [NewRelicName("CustomPassword")]
    public string Password { get; set; }    // Sent as CustomPassword with event.
}
```