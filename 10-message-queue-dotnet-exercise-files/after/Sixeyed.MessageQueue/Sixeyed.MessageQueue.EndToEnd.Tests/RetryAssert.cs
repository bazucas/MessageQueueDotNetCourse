using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;

namespace Sixeyed.MessageQueue.EndToEnd.Tests
{
    class RetryAssert
    {
        public static void WithinTimeout(Func<bool> assertion, int retryIntervalMilliseconds, int timeoutMilliseconds, string failureMessage, params object[] messageArgs)
        {
            var assert = false;
            var timeoutElapsed = false;
            var stopwatch = Stopwatch.StartNew();
            while (!assert && !timeoutElapsed)
            {
                assert = assertion();
                timeoutElapsed = (stopwatch.Elapsed.TotalMilliseconds >= timeoutMilliseconds);
                if (assert)
                {
                    break;
                }
                if (timeoutElapsed)
                {
                    Assert.Fail(failureMessage, messageArgs);
                }
                else
                {
                    Thread.Sleep(retryIntervalMilliseconds);
                }
            }
            Assert.IsTrue(assert, failureMessage);
        }
    }
}
