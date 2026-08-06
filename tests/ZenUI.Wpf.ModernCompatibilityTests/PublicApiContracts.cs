using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;

using ZenUI.Wpf.Controls;

namespace ZenUI.Wpf.ModernCompatibilityTests
{
    internal static class PublicApiContracts
    {
        private const BindingFlags DeclaredMembers =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance |
            BindingFlags.Static | BindingFlags.DeclaredOnly;

        private const BindingFlags DeclaredStaticFields =
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        public static void Verify()
        {
            var failures = new List<string>();
            var assembly = typeof(ZenButton).Assembly;

            ContractAssert.IsNotNull(
                assembly.GetType("ZenUI.Wpf.Controls.ZenDateTimePicker"),
                "ZenUI.Wpf 程序集缺少 ZenDateTimePicker。");
            ContractAssert.IsNotNull(
                typeof(ZenUI.Wpf.Converters.BoolToVisibilityConverter).Assembly.GetType(
                    "ZenUI.Wpf.Converters.ComparisonToVisibilityConverter"),
                "ZenUI.Wpf.Converters 程序集缺少 ComparisonToVisibilityConverter。");

            foreach (var type in assembly.GetTypes()
                .Where(type => type.Namespace != null &&
                    type.Namespace.StartsWith("ZenUI.Wpf", StringComparison.Ordinal))
                .OrderBy(type => type.FullName, StringComparer.Ordinal))
            {
                AuditDependencyProperties(type, failures);
                AuditDependencyPropertyKeys(type, failures);
                AuditRoutedEvents(type, failures);
            }

            ContractAssert.AreEqual(
                0,
                failures.Count,
                "WPF 公共 API 注册约定不兼容：" + Environment.NewLine +
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
                    failures.Add(ownerType.FullName + "." + field.Name + " 为空。");
                    continue;
                }

                if (!string.Equals(
                    field.Name,
                    dependencyProperty.Name + "Property",
                    StringComparison.Ordinal))
                {
                    failures.Add(ownerType.FullName + "." + field.Name + " 命名不匹配。");
                }

                var wrapper = ownerType.GetProperty(dependencyProperty.Name, DeclaredMembers);
                var getter = FindAttachedGetter(ownerType, dependencyProperty);
                var setter = FindAttachedSetter(ownerType, dependencyProperty);
                if (wrapper != null && wrapper.PropertyType != dependencyProperty.PropertyType)
                {
                    failures.Add(ownerType.FullName + "." + wrapper.Name + " 类型不匹配。");
                }
                if (getter != null && getter.ReturnType != dependencyProperty.PropertyType)
                {
                    failures.Add(ownerType.FullName + "." + getter.Name + " 返回类型不匹配。");
                }
                if (setter != null &&
                    setter.GetParameters()[1].ParameterType != dependencyProperty.PropertyType)
                {
                    failures.Add(ownerType.FullName + "." + setter.Name + " 参数类型不匹配。");
                }
                if (field.IsPublic && wrapper == null && getter == null && setter == null)
                {
                    failures.Add(ownerType.FullName + "." + field.Name + " 缺少 CLR 包装器或附加属性访问器。");
                }
                if ((getter == null) != (setter == null))
                {
                    failures.Add(ownerType.FullName + "." + dependencyProperty.Name + " 的附加属性访问器不成对。");
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
                    failures.Add(ownerType.FullName + "." + field.Name + " 为空。");
                    continue;
                }

                var propertyName = key.DependencyProperty.Name;
                var propertyField = ownerType.GetField(
                    propertyName + "Property",
                    DeclaredStaticFields);
                if (!string.Equals(
                        field.Name,
                        propertyName + "PropertyKey",
                        StringComparison.Ordinal) ||
                    propertyField == null ||
                    !ReferenceEquals(propertyField.GetValue(null), key.DependencyProperty))
                {
                    failures.Add(ownerType.FullName + "." + field.Name + " 缺少匹配的只读依赖属性。");
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
                    failures.Add(ownerType.FullName + "." + field.Name + " 为空。");
                    continue;
                }

                var clrEvent = ownerType.GetEvent(routedEvent.Name, DeclaredMembers);
                if (!string.Equals(
                        field.Name,
                        routedEvent.Name + "Event",
                        StringComparison.Ordinal) ||
                    routedEvent.OwnerType != ownerType ||
                    (field.IsPublic && clrEvent == null) ||
                    (clrEvent != null && clrEvent.EventHandlerType != routedEvent.HandlerType))
                {
                    failures.Add(ownerType.FullName + "." + field.Name + " 路由事件契约不匹配。");
                }
            }
        }

        private static MethodInfo FindAttachedGetter(
            Type ownerType,
            DependencyProperty dependencyProperty)
        {
            return ownerType.GetMethod(
                "Get" + dependencyProperty.Name,
                DeclaredMembers,
                null,
                new[] { typeof(DependencyObject) },
                null);
        }

        private static MethodInfo FindAttachedSetter(
            Type ownerType,
            DependencyProperty dependencyProperty)
        {
            return ownerType.GetMethod(
                "Set" + dependencyProperty.Name,
                DeclaredMembers,
                null,
                new[] { typeof(DependencyObject), dependencyProperty.PropertyType },
                null);
        }
    }
}
