using System;

namespace NewRelic.CustomEvents.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class NewRelicIgnore : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class NewRelicName : Attribute
    {
        public NewRelicName(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}