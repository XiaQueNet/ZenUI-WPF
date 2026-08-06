using System;
using System.Collections.Generic;

namespace ZenUI.Wpf.ModernCompatibilityTests
{
    internal static class Program
    {
        [STAThread]
        private static int Main()
        {
            var contracts = new[]
            {
                new ContractCase("程序集与公共 WPF API", PublicApiContracts.Verify),
                new ContractCase("主题与密度资源", ThemeAndControlContracts.VerifyThemesAndDensities),
                new ContractCase("控件样式与模板", ThemeAndControlContracts.VerifyControlStylesAndTemplates),
                new ContractCase("转换器行为", ConverterContracts.Verify)
            };
            var failures = new List<string>();

            Console.WriteLine(
                "ZenUI WPF compatibility contracts on {0} ({1})",
                System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                Environment.Version);

            foreach (var contract in contracts)
            {
                try
                {
                    contract.Execute();
                    Console.WriteLine("[PASS] {0}", contract.Name);
                }
                catch (Exception exception)
                {
                    failures.Add(contract.Name + Environment.NewLine + exception);
                    Console.Error.WriteLine("[FAIL] {0}", contract.Name);
                    Console.Error.WriteLine(exception);
                }
            }

            Console.WriteLine(
                "Compatibility contracts: {0} passed, {1} failed.",
                contracts.Length - failures.Count,
                failures.Count);
            return failures.Count == 0 ? 0 : 1;
        }

        private sealed class ContractCase
        {
            public ContractCase(string name, Action execute)
            {
                Name = name;
                Execute = execute;
            }

            public string Name { get; }

            public Action Execute { get; }
        }
    }
}
