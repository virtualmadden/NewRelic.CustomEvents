using System;
using NewRelic.Service.CustomEvents.Enumerations;

namespace NewRelic.Service.CustomEvents.Events
{
    public class ServiceEvent : BaseEvent
    {
        public ServiceEvent(string serviceName, Guid? correlationId = null) : base(serviceName, correlationId)
        {
        }

        public string OperationName { get; set; }
        public OperationLevel OperationLevel { get; set; }
        public bool OperationSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
}