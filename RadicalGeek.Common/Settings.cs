using System.Configuration;
using System.Reflection;
using Microsoft.Win32;

namespace RadicalGeek.Common
{
    /// <summary>
    /// Manages settings. This class promotes the use of Environment Variables. The first time a Setting is used, it is placed in the Environment Variables of the host machine. From that point on, the Environment Variable will be used instead of the App Config or Web Config. The App Config or Web Config then becomes a container ONLY for the DEFAULT settings.
    /// </summary>
    public static class Settings
    {
        private const string RegistryKeyName = @"System\CurrentControlSet\Control\Session Manager\Environment";

        public static string GetConnectionString(string connectionStringName)
        {
            AssemblyName assemblyName = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName();

            string environmentVariable = string.Format("{0}_ConnectionString_{1}", assemblyName.Name,
                                                       connectionStringName);

            string variable = GetEnvironmentVariable(environmentVariable);
            if (variable == null)
            {
                ConnectionStringSettings connectionStringSettings =
                        ConfigurationManager.ConnectionStrings[connectionStringName];
                variable = connectionStringSettings.ConnectionString;
                SetEnvironmentVariable(environmentVariable, variable);
            }
            return variable;
        }

        public static string GetAppSetting(string appSetting)
        {
            AssemblyName assemblyName = (Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly()).GetName();

            string environmentVariable = string.Format("{0}_Setting_{1}", assemblyName.Name,
                                                       appSetting);

            string variable = GetEnvironmentVariable(environmentVariable);
            if (variable == null)
            {
                variable = ConfigurationManager.AppSettings[appSetting];
                SetEnvironmentVariable(environmentVariable, variable);
            }
            return variable;
        }

        private static string GetEnvironmentVariable(string variable)
        {
            using (RegistryKey environmentKey =
                       Registry.LocalMachine.OpenSubKey(RegistryKeyName, false))
            {
                if (environmentKey == null)
                    return null;
                return environmentKey.GetValue(variable) as string;
            }
        }

        private static void SetEnvironmentVariable(string variable, string value)
        {
            using (RegistryKey environmentKey = Registry.LocalMachine.OpenSubKey(RegistryKeyName, true))
                if (environmentKey != null)
                    if (value == null)
                        environmentKey.DeleteValue(variable, false);
                    else
                        environmentKey.SetValue(variable, value);
        }

    }
}
