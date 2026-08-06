using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ZenUI.Wpf.Controls;

namespace ZenUI.Wpf.Tests.Controls
{
    [TestClass]
    public class PublicApiConventionTests
    {
        private const BindingFlags DeclaredStaticFields =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;

        private const BindingFlags DeclaredMembers =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
            BindingFlags.Static | BindingFlags.DeclaredOnly;

        [TestMethod]
        public void WpfRegistrationMembersFollowNamingAndTypeConventions()
        {
            var failures = new List<string>();
            var assemblyTypes = typeof(ZenButton).Assembly.GetTypes()
                .Where(type => type.Namespace != null && type.Namespace.StartsWith("ZenUI.Wpf", StringComparison.Ordinal))
                .OrderBy(type => type.FullName, StringComparer.Ordinal);

            foreach (var type in assemblyTypes)
            {
                AuditDependencyProperties(type, failures);
                AuditDependencyPropertyKeys(type, failures);
                AuditRoutedEvents(type, failures);
                AuditPublicProperties(type, failures);
            }

            Assert.AreEqual(
                0,
                failures.Count,
                "Public API convention violations:" + Environment.NewLine +
                string.Join(Environment.NewLine, failures.Select(failure => "- " + failure)));
        }

        private static void AuditDependencyProperties(Type ownerType, List<string> failures)
        {
            foreach (var field in ownerType.GetFields(DeclaredStaticFields)
                .Where(field => field.FieldType == typeof(DependencyProperty)))
            {
                var dependencyProperty = field.GetValue(null) as DependencyProperty;
                if (dependencyProperty == null)
                {
                    failures.Add($"{ownerType.FullName}.{field.Name} is null.");
                    continue;
                }

                var expectedFieldName = dependencyProperty.Name + "Property";
                if (!string.Equals(field.Name, expectedFieldName, StringComparison.Ordinal))
                {
                    failures.Add(
                        $"{ownerType.FullName}.{field.Name} must be named {expectedFieldName}.");
                }

                var wrapper = ownerType.GetProperty(dependencyProperty.Name, DeclaredMembers);
                var getter = FindAttachedGetter(ownerType, dependencyProperty);
                var setter = FindAttachedSetter(ownerType, dependencyProperty);

                if (wrapper != null && wrapper.PropertyType != dependencyProperty.PropertyType)
                {
                    failures.Add(
                        $"{ownerType.FullName}.{wrapper.Name} has type {wrapper.PropertyType.FullName}; " +
                        $"the registered property type is {dependencyProperty.PropertyType.FullName}.");
                }

                if (getter != null && getter.ReturnType != dependencyProperty.PropertyType)
                {
                    failures.Add(
                        $"{ownerType.FullName}.{getter.Name} returns {getter.ReturnType.FullName}; " +
                        $"the registered property type is {dependencyProperty.PropertyType.FullName}.");
                }

                if (setter != null && setter.GetParameters()[1].ParameterType != dependencyProperty.PropertyType)
                {
                    failures.Add(
                        $"{ownerType.FullName}.{setter.Name} accepts " +
                        $"{setter.GetParameters()[1].ParameterType.FullName}; the registered property type is " +
                        $"{dependencyProperty.PropertyType.FullName}.");
                }

                if (field.IsPublic && wrapper == null && getter == null && setter == null)
                {
                    failures.Add(
                        $"{ownerType.FullName}.{field.Name} has no CLR wrapper or attached-property accessors.");
                }

                if ((getter == null) != (setter == null))
                {
                    failures.Add(
                        $"{ownerType.FullName}.{dependencyProperty.Name} must expose both Get and Set accessors.");
                }
            }
        }

        private static void AuditDependencyPropertyKeys(Type ownerType, List<string> failures)
        {
            foreach (var field in ownerType.GetFields(DeclaredStaticFields)
                .Where(field => field.FieldType == typeof(DependencyPropertyKey)))
            {
                var key = field.GetValue(null) as DependencyPropertyKey;
                if (key == null)
                {
                    failures.Add($"{ownerType.FullName}.{field.Name} is null.");
                    continue;
                }

                var propertyName = key.DependencyProperty.Name;
                if (!string.Equals(field.Name, propertyName + "PropertyKey", StringComparison.Ordinal))
                {
                    failures.Add(
                        $"{ownerType.FullName}.{field.Name} must be named {propertyName}PropertyKey.");
                }

                var propertyField = ownerType.GetField(propertyName + "Property", DeclaredStaticFields);
                if (propertyField == null ||
                    !ReferenceEquals(propertyField.GetValue(null), key.DependencyProperty))
                {
                    failures.Add(
                        $"{ownerType.FullName}.{field.Name} has no matching {propertyName}Property field.");
                }
            }
        }

        private static void AuditRoutedEvents(Type ownerType, List<string> failures)
        {
            foreach (var field in ownerType.GetFields(DeclaredStaticFields)
                .Where(field => field.FieldType == typeof(RoutedEvent)))
            {
                var routedEvent = field.GetValue(null) as RoutedEvent;
                if (routedEvent == null)
                {
                    failures.Add($"{ownerType.FullName}.{field.Name} is null.");
                    continue;
                }

                if (!string.Equals(field.Name, routedEvent.Name + "Event", StringComparison.Ordinal))
                {
                    failures.Add(
                        $"{ownerType.FullName}.{field.Name} must be named {routedEvent.Name}Event.");
                }

                if (routedEvent.OwnerType != ownerType)
                {
                    failures.Add(
                        $"{ownerType.FullName}.{field.Name} is registered for owner {routedEvent.OwnerType.FullName}.");
                }

                var clrEvent = ownerType.GetEvent(routedEvent.Name, DeclaredMembers);
                if (field.IsPublic && clrEvent == null)
                {
                    failures.Add(
                        $"{ownerType.FullName}.{field.Name} has no matching CLR event {routedEvent.Name}.");
                }
                else if (clrEvent != null && clrEvent.EventHandlerType != routedEvent.HandlerType)
                {
                    failures.Add(
                        $"{ownerType.FullName}.{clrEvent.Name} uses {clrEvent.EventHandlerType.FullName}; " +
                        $"the routed event uses {routedEvent.HandlerType.FullName}.");
                }
            }
        }

        private static void AuditPublicProperties(Type ownerType, List<string> failures)
        {
            foreach (var property in ownerType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (property.Name.Length == 0 || !char.IsUpper(property.Name[0]))
                {
                    failures.Add($"{ownerType.FullName}.{property.Name} must use PascalCase.");
                }

                var baseProperty = ownerType.BaseType?.GetProperty(
                    property.Name,
                    BindingFlags.Public | BindingFlags.Instance);
                if (baseProperty != null && property.GetBaseDefinition() == property)
                {
                    failures.Add(
                        $"{ownerType.FullName}.{property.Name} hides the public property " +
                        $"{baseProperty.DeclaringType.FullName}.{baseProperty.Name}.");
                }
            }
        }

        private static MethodInfo FindAttachedGetter(Type ownerType, DependencyProperty dependencyProperty)
        {
            return ownerType.GetMethod(
                "Get" + dependencyProperty.Name,
                DeclaredMembers,
                null,
                new[] { typeof(DependencyObject) },
                null);
        }

        private static MethodInfo FindAttachedSetter(Type ownerType, DependencyProperty dependencyProperty)
        {
            return ownerType.GetMethod(
                "Set" + dependencyProperty.Name,
                DeclaredMembers,
                null,
                new[] { typeof(DependencyObject), dependencyProperty.PropertyType },
                null);
        }
    }

    internal static class PropertyInfoExtensions
    {
        public static PropertyInfo GetBaseDefinition(this PropertyInfo property)
        {
            var accessor = property.GetMethod ?? property.SetMethod;
            var baseAccessor = accessor?.GetBaseDefinition();
            if (accessor == null || baseAccessor == accessor)
            {
                return property;
            }

            return baseAccessor.DeclaringType?.GetProperty(
                property.Name,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}
