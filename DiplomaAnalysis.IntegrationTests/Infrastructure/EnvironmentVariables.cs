namespace DiplomaAnalysis.IntegrationTests.Infrastructure
{
    public static class EnvironmentVariables
    {
        private const string ProductiveDataDecryptionKeyVar = "PRODUCTIVE_DATA_DECRYPTION_KEY";
        private const string ApplicationPathVar = "APPLICATION_PATH";

        public static string ProductiveDataDecryptionKey => Environment.GetEnvironmentVariable(ProductiveDataDecryptionKeyVar);

        public static string ApplicationPath => Environment.GetEnvironmentVariable(ApplicationPathVar);
    }
}
