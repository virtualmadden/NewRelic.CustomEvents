using System;
using System.Linq;
using FluentAssertions;
using NewRelic.CustomEvents.Enumerations;
using NewRelic.CustomEvents.Events;
using NewRelic.CustomEvents.Exceptions;
using NUnit.Framework;

namespace NewRelic.CustomEvents.Tests.Events
{
    [TestFixture]
    public class ServiceEventTests
    {
        [SetUp]
        public void SetUp()
        {
            _serviceEvent = new ServiceEvent("CustomEventApplication");
        }

        private ServiceEvent _serviceEvent;

        [TestCase("Float", 5.6f)]
        [TestCase("String", "test")]
        [TestCase("Int", 20)]
        public void ShouldRetainPropertyType(string name, object value)
        {
            _serviceEvent.CustomParameters.Add(name, value);

            var newRelicEvent = _serviceEvent.ConvertToCustomEvent();

            newRelicEvent.Any(x => x.Key.Equals(name)).Should().BeTrue();
            newRelicEvent.First(x => x.Key.Equals(name)).Value.Should().BeOfType(value.GetType());
            newRelicEvent.First(x => x.Key.Equals(name)).Value.Should().Be(value);
        }

        [Test]
        public void ShouldAllowForDuplicateKeys()
        {
            _serviceEvent.OperationName = "Foo";
            _serviceEvent.CustomParameters.Add("OperationName", "Bar");

            Action action = () => _serviceEvent.ConvertToCustomEvent();

            action.Should().Throw<DuplicateKeyException>();
        }

        [Test]
        public void ShouldIgnoreNullValues()
        {
            _serviceEvent.OperationName = null;

            var newRelicEvent = _serviceEvent.ConvertToCustomEvent();

            newRelicEvent.Any(x => x.Key.Equals("OperationName")).Should().BeFalse();
        }

        [Test]
        public void ShouldPopulateEventBasedOnSuppliedProperties()
        {
            _serviceEvent.OperationName = "Foo";
            _serviceEvent.OperationLevel = OperationLevel.Other;
            _serviceEvent.OperationSuccess = true;

            var newRelicEvent = _serviceEvent.ConvertToCustomEvent();

            newRelicEvent.Any(x => x.Key.Equals(nameof(_serviceEvent.CorrelationId))).Should().BeTrue();
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.CorrelationId))).Value.Should().BeOfType(_serviceEvent.CorrelationId.GetType());
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.CorrelationId))).Value.Should().Be(_serviceEvent.CorrelationId);

            newRelicEvent.Any(x => x.Key.Equals(nameof(_serviceEvent.ServiceName))).Should().BeTrue();
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.ServiceName))).Value.Should().BeOfType(_serviceEvent.ServiceName.GetType());
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.ServiceName))).Value.Should().Be(_serviceEvent.ServiceName);

            newRelicEvent.Any(x => x.Key.Equals(nameof(_serviceEvent.OperationName))).Should().BeTrue();
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.OperationName))).Value.Should().BeOfType(_serviceEvent.OperationName.GetType());
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.OperationName))).Value.Should().Be(_serviceEvent.OperationName);

            newRelicEvent.Any(x => x.Key.Equals(nameof(_serviceEvent.OperationLevel))).Should().BeTrue();
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.OperationLevel))).Value.Should().BeOfType(_serviceEvent.OperationLevel.GetType());
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.OperationLevel))).Value.Should().Be(_serviceEvent.OperationLevel);

            newRelicEvent.Any(x => x.Key.Equals(nameof(_serviceEvent.OperationSuccess))).Should().BeTrue();
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.OperationSuccess))).Value.Should().BeOfType(_serviceEvent.OperationSuccess.GetType());
            newRelicEvent.First(x => x.Key.Equals(nameof(_serviceEvent.OperationSuccess))).Value.Should().Be(_serviceEvent.OperationSuccess);
        }

        [Test]
        public void ShouldRetainCustomParameters()
        {
            _serviceEvent.CustomParameters.Add("Foo", "Bar");

            var newRelicEvent = _serviceEvent.ConvertToCustomEvent();

            newRelicEvent.Any(x => x.Key.Equals("Foo")).Should().BeTrue();
            newRelicEvent.First(x => x.Key.Equals("Foo")).Value.Should().BeOfType("Bar".GetType());
            newRelicEvent.First(x => x.Key.Equals("Foo")).Value.Should().Be("Bar");
        }
    }
}