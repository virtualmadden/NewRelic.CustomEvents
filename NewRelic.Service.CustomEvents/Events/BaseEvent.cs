using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NewRelic.Service.CustomEvents.Attributes;
using NewRelic.Service.CustomEvents.Exceptions;

namespace NewRelic.Service.CustomEvents.Events
{
    public abstract class BaseEvent
    {
        public Dictionary<string, object> CustomParameters = new Dictionary<string, object>();

        protected BaseEvent(string serviceName, Guid? correlationId = null)
        {
            CorrelationId = correlationId ?? Guid.NewGuid();
            ServiceName = serviceName;
        }

        public Guid CorrelationId { get; }
        public string ServiceName { get; }

        internal Dictionary<string, object> ConvertToCustomEvent()
        {
            var eventParameters = GetType().GetProperties()
                .Where(properties => properties.GetValue(this) != null && !properties.IsDefined(typeof(NewRelicIgnore)))
                .ToDictionary(
                    properties => properties.IsDefined(typeof(NewRelicName))
                        ? properties.GetCustomAttribute<NewRelicName>().Name
                        : properties.Name, properties => properties.GetValue(this));

            foreach (var parameter in CustomParameters)
            {
                if (eventParameters.ContainsKey(parameter.Key))
                {
                    throw new DuplicateKeyException($"The {GetType().Name} already contains the key {parameter.Key}.");
                }

                eventParameters.Add(parameter.Key, parameter.Value);
            }

            return eventParameters;
        }

        public void SendAsync()
        {
            Task.Run(() => Api.Agent.NewRelic.RecordCustomEvent(GetEventType(), ConvertToCustomEvent())).ConfigureAwait(false);
        }

        internal string GetEventType()
        {
            var eventType = GetType();

            return eventType.IsDefined(typeof(NewRelicName))
                ? eventType.GetCustomAttribute<NewRelicName>().Name
                : eventType.Name;
        }
    }
}