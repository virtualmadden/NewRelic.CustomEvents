using System;
using System.Linq;
using FluentAssertions;
using NewRelic.Service.CustomEvents.Attributes;
using NewRelic.Service.CustomEvents.Events;
using NUnit.Framework;

namespace NewRelic.Service.CustomEvents.Tests.Attributes
{
    [TestFixture]
    public class EventAttributesTests
    {
        [SetUp]
        public void SetUp()
        {
            _testEvent = new TestEvent("CustomTestEvent")
            {
                Username = "jondoe",
                Password = "notMyP@$$word"
            };
        }

        private TestEvent _testEvent;

        [Test]
        public void ShouldIgnorePropertiesWithAttribute()
        {
            var newRelicEvent = _testEvent.ConvertToCustomEvent();

            newRelicEvent.First(x => x.Key.Equals(nameof(_testEvent.ServiceName))).Value.Should().Be("CustomTestEvent");

            newRelicEvent.Any(x => x.Key.Equals(nameof(_testEvent.Username))).Should().BeFalse();
        }

        [Test]
        public void ShouldNameClassWithAttribute()
        {
            new TestEvent("CustomTestEvent").GetEventType().Should().Be("TestEvent");
            new RenamedEvent("CustomTestEvent").GetEventType().Should().Be("TestEvent");
        }

        [Test]
        public void ShouldNamePropertiesWithAttribute()
        {
            var newRelicEvent = _testEvent.ConvertToCustomEvent();

            newRelicEvent.First(x => x.Key.Equals(nameof(_testEvent.ServiceName))).Value.Should().Be("CustomTestEvent");

            newRelicEvent.Any(x => x.Key.Equals(nameof(_testEvent.Password))).Should().BeFalse();

            newRelicEvent.Any(x => x.Key.Equals("NotPassword")).Should().BeTrue();
            newRelicEvent.First(x => x.Key.Equals("NotPassword")).Value.Should().Be("notMyP@$$word");
        }
    }

    internal class TestEvent : BaseEvent
    {
        public TestEvent(string applicationName, Guid? requestId = null) : base(applicationName, requestId)
        {
        }

        [NewRelicIgnore] public string Username { get; set; }

        [NewRelicName("NotPassword")] public string Password { get; set; }
    }

    [NewRelicName("TestEvent")]
    internal class RenamedEvent : BaseEvent
    {
        public RenamedEvent(string applicationName, Guid? requestId = null) : base(applicationName, requestId)
        {
        }
    }
}