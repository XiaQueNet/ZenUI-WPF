using System;
using System.Collections.Generic;

namespace ZenUI.Wpf.ModernCompatibilityTests
{
    internal static class ContractAssert
    {
        public static void AreEqual<T>(T expected, T actual, string message)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new InvalidOperationException(
                    $"{message} Expected: {expected}; actual: {actual}.");
            }
        }

        public static void IsNotNull(object value, string message)
        {
            if (value == null)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void IsTrue(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}
