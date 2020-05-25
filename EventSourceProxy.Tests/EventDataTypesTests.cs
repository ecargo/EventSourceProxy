using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using NUnit.Framework;

namespace EventSourceProxy.Tests
{
    [TestFixture]
    public class EventDataTypesTests
    {
        #region Tests for Built-in Types

        public enum FooEnum
        {
            Foo,
            Bar
        }

        public interface ITypeLog<T>
        {
            void Log(T t);
        }

        public static class TypeLogTester<T>
        {
            public static void Test(T t)
            {
                using (var testLog = (EventSource) EventSourceImplementer.GetEventSourceAs<ITypeLog<T>>())
                using (var listener = new TestEventListener())
                {
                    listener.EnableEvents(testLog, EventLevel.LogAlways);

                    var tLog = (ITypeLog<T>) testLog;
                    tLog.Log(t);

                    var value = listener.Events.Last().Payload[0];
                    if (TypeIsSupportedByEventSource(typeof(T)))
                        Assert.AreEqual(t, value);
                    else
                        Assert.AreEqual(t.ToString(), value);

                    listener.DisableEvents(testLog);
                }
            }

            internal static bool TypeIsSupportedByEventSource(Type type)
            {
                if (type == typeof(string)) return true;
                if (type == typeof(int)) return true;
                if (type == typeof(long)) return true;
                if (type == typeof(ulong)) return true;
                if (type == typeof(byte)) return true;
                if (type == typeof(sbyte)) return true;
                if (type == typeof(short)) return true;
                if (type == typeof(ushort)) return true;
                if (type == typeof(float)) return true;
                if (type == typeof(double)) return true;
                if (type == typeof(bool)) return true;
                if (type == typeof(Guid)) return true;
                if (type.IsEnum) return true;

                return false;
            }
        }

        static IEnumerable<TestCaseData> GetSupportedTypeTestCases()
        {
            yield return new TestCaseData(new Action(() => TypeLogTester<string>.Test("string")));
            yield return new TestCaseData(new Action(() => TypeLogTester<int>.Test(5)));
            yield return new TestCaseData(new Action(() => TypeLogTester<long>.Test(0x800000000)));
            yield return new TestCaseData(new Action(() => TypeLogTester<ulong>.Test(0x1800000000)));
            yield return new TestCaseData(new Action(() => TypeLogTester<byte>.Test(0x78)));
            yield return new TestCaseData(new Action(() => TypeLogTester<sbyte>.Test(0x20)));
            yield return new TestCaseData(new Action(() => TypeLogTester<short>.Test(0x1001)));
            yield return new TestCaseData(new Action(() => TypeLogTester<ushort>.Test(0x8010)));
            yield return new TestCaseData(new Action(() => TypeLogTester<float>.Test(1.234f)));
            yield return new TestCaseData(new Action(() => TypeLogTester<double>.Test(2.3456)));
            yield return new TestCaseData(new Action(() => TypeLogTester<bool>.Test(true)));
            yield return new TestCaseData(new Action(() => TypeLogTester<Guid>.Test(Guid.NewGuid())));
            yield return new TestCaseData(new Action(() => TypeLogTester<FooEnum>.Test(FooEnum.Bar)));
            yield return new TestCaseData(new Action(() => TypeLogTester<char>.Test('c')));
            yield return new TestCaseData(new Action(() => TypeLogTester<decimal>.Test(3.456m)));
            yield return new TestCaseData(new Action(() => TypeLogTester<IntPtr>.Test(new IntPtr(1234))));
            yield return new TestCaseData(new Action(() => TypeLogTester<DateTime>.Test(DateTime.Now)));
        }

        [TestCaseSource(nameof(GetSupportedTypeTestCases))]
        public void When_logging_supported_types_Then_the_value_is_logged_successfully(Action act)
        {
            Assert.DoesNotThrow(() => act());
        }

        static IEnumerable<TestCaseData> GetUnsupportedTypeTestCases()
        {
            yield return new TestCaseData(new Action(() =>TypeLogTester<int?>.Test(5)));
            yield return new TestCaseData(new Action(() =>TypeLogTester<int?>.Test(null)));
        }

        [TestCaseSource(nameof(GetUnsupportedTypeTestCases))]
        public void When_logging_unsupported_types_Then_a_NotSupportedException_is_thrown(Action act)
        {
            Assert.Throws<NotSupportedException>(() => act());
        }

        #endregion

        #region Serialized Types in Abstract Methods Tests
        public abstract class TypeLogWithSerializedTypesInAbstractMethod : EventSource
        {
            public abstract void LogIntPtr(IntPtr p);
            public abstract void LogChar(char c);
            public abstract void LogDecimal(decimal d);
        }

        [Test]
        public void BuiltInSerializedTypesCanBeLoggedInAbstractMethods()
        {
            var listener = new TestEventListener();
            var testLog = EventSourceImplementer.GetEventSourceAs<TypeLogWithSerializedTypesInAbstractMethod>();
            listener.EnableEvents(testLog, EventLevel.LogAlways);

            testLog.LogIntPtr(new IntPtr(1234)); Assert.AreEqual("1234", listener.Events.Last().Payload[0].ToString());
            testLog.LogChar('c'); Assert.AreEqual("c", listener.Events.Last().Payload[0].ToString());
            testLog.LogDecimal(3.456m); Assert.AreEqual("3.456", listener.Events.Last().Payload[0].ToString());
        }
        #endregion

        #region Serialized Types in Direct Methods Tests
        public class TypeLogWithSerializedTypesInDirectMethod : EventSource
        {
            public void LogIntPtr(IntPtr p) { WriteEvent(1, p); }
            public void LogChar(char c) { WriteEvent(2, c); }
            public void LogDecimal(decimal d) { WriteEvent(3, d); }
        }

        [Test]
        public void BuiltInSerializedTypesCanBeLoggedInDirectMethods()
        {
            var listener = new TestEventListener();
            var testLog = EventSourceImplementer.GetEventSourceAs<TypeLogWithSerializedTypesInDirectMethod>();
            listener.EnableEvents(testLog, EventLevel.LogAlways);

            testLog.LogIntPtr(new IntPtr(1234)); Assert.AreEqual("1234", listener.Events.Last().Payload[0].ToString());
            testLog.LogChar('c'); Assert.AreEqual("c", listener.Events.Last().Payload[0].ToString());
            testLog.LogDecimal(3.456m); Assert.AreEqual("3.456", listener.Events.Last().Payload[0].ToString());
        }
        #endregion
    }
}
