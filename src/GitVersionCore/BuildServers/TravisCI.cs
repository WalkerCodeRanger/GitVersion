using GitVersion.OutputVariables;
using GitVersion.Common;

namespace GitVersion.BuildServers
{
    public class TravisCI : BuildServerBase
    {
        public TravisCI(IEnvironment environment) : base(environment)
        {
        }

        public const string EnvironmentVariableName = "TRAVIS";
        protected override string EnvironmentVariable { get; } = EnvironmentVariableName;

        public override bool CanApplyToCurrentContext ()
        {
            return "true".Equals(Environment.GetEnvironmentVariable(EnvironmentVariable)) && "true".Equals(Environment.GetEnvironmentVariable("CI"));
        }

        public override string GenerateSetVersionMessage(VersionVariables variables)
        {
            return variables.FullSemVer;
        }

        public override string[] GenerateSetParameterMessage(string name, string value)
        {
            return new[]
            {
                $"GitVersion_{name}={value}"
            };
        }

        public override bool PreventFetch () => true;
    }
}

