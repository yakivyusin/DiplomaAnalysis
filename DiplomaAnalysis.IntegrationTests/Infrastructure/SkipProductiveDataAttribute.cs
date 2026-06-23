namespace DiplomaAnalysis.IntegrationTests.Infrastructure;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class SkipProductiveDataAttribute : SkipAttribute
{
    public SkipProductiveDataAttribute() : base("Productive")
    {
    }

    public override Task<bool> ShouldSkip(TestRegisteredContext context) => Task.FromResult(string.IsNullOrEmpty(EnvironmentVariables.ProductiveDataDecryptionKey));
}
