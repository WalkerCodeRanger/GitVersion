using System.IO;
using NUnit.Framework;
using Shouldly;
using GitVersion.BuildServers;
using GitVersion.Common;

namespace GitVersionCore.Tests.BuildServers
{
    [TestFixture]
    public class EnvRunTests : TestBase
    {
        private const string EnvVarName = "ENVRUN_DATABASE";
        private string mFilePath;
        private IEnvironment environment;

        [SetUp]
        public void SetEnvironmentVariableForTest()
        {
            environment = new TestEnvironment();
            // set environment variable and create an empty envrun file to indicate that EnvRun is running...
            mFilePath = Path.Combine(Path.GetTempPath(), "envrun.db");
            environment.SetEnvironmentVariable(EnvVarName, mFilePath);
            File.OpenWrite(mFilePath).Dispose();
        }

        [TearDown]
        public void ClearEnvironmentVariableForTest()
        {
            environment.SetEnvironmentVariable(EnvVarName, null);
            File.Delete(mFilePath);
        }

        [Test]
        public void CanApplyToCurrentContext()
        {
            EnvRun envrun = new EnvRun(environment);
            bool applys = envrun.CanApplyToCurrentContext();
            applys.ShouldBeTrue();
        }

        [Test]
        public void CanApplyToCurrentContext_EnvironmentVariableNotSet()
        {
            environment.SetEnvironmentVariable(EnvVarName, null);
            EnvRun envrun = new EnvRun(environment);
            bool applys = envrun.CanApplyToCurrentContext();
            applys.ShouldBeFalse();
        }

        [TestCase("1.2.3")]
        [TestCase("1.2.3-rc4")]
        public void GenerateSetVersionMessage(string fullSemVer)
        {
            EnvRun envrun = new EnvRun(environment);
            var vars = new TestableVersionVariables(fullSemVer: fullSemVer);
            var version = envrun.GenerateSetVersionMessage(vars);
            version.ShouldBe(fullSemVer);
        }

        [TestCase("Version", "1.2.3",     "@@envrun[set name='GitVersion_Version' value='1.2.3']")]
        [TestCase("Version", "1.2.3-rc4", "@@envrun[set name='GitVersion_Version' value='1.2.3-rc4']")]
        public void GenerateSetParameterMessage(string name, string value, string expected)
        {
            EnvRun envrun = new EnvRun(environment);
            var output = envrun.GenerateSetParameterMessage(name, value);
            output.ShouldHaveSingleItem();
            output[0].ShouldBe(expected);
        }

    }
}
