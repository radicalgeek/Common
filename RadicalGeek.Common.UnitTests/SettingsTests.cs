using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RadicalGeek.Common.UnitTests
{
    [TestClass]
    public class SettingsTests
    {
        [TestMethod]
        public void TestSettingIsReadFromConfigFileAndAppliedCorrectly()
        {
            string variable = "Dollar.Common.UnitTests_Setting_Test";
            try
            {
                Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Machine);
                Assert.AreEqual(null, Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine));
                string expected = "Test Setting";
                Assert.AreEqual(expected, Settings.GetAppSetting("Test"));
                Assert.AreEqual(expected,
                                Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine));
            }
            finally
            {
                Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Machine);
            }
        }

        [TestMethod]
        public void TestConnectionStringIsReadFromConfigFileAndAppliedCorrectly()
        {
            string variable = "Dollar.Common.UnitTests_ConnectionString_Test";
            try
            {
                Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Machine);
                Assert.AreEqual(null, Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine));
                string expected = "Test ConnectionString";
                Assert.AreEqual(expected, Settings.GetConnectionString("Test"));
                Assert.AreEqual(expected,
                                Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine));
            }
            finally
            {
                Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Machine);
            }
        }
    }
}
