using DiplomaAnalysis.IntegrationTests.Infrastructure;

namespace DiplomaAnalysis.IntegrationTests
{
    public class ProductiveDataTests
    {
        private static readonly AnalysisServiceClient _analysisServiceClient = new()
        {
            FileProvider = new(decryptFiles: true)
        };

        public ProductiveDataTests()
        {
            Skip.If(string.IsNullOrEmpty(EnvironmentVariables.ProductiveDataDecryptionKey));
        }

        [SkippableTheory]
        [InlineData("1.docx.aes", 0)]
        [InlineData("2.docx.aes", 0)]
        [InlineData("3.docx.aes", 2)]
        [InlineData("4.docx.aes", 0)]
        [InlineData("5.docx.aes", 1)]
        [InlineData("6.docx.aes", 0)]
        [InlineData("7.docx.aes", 0)]
        public async void CharReplacement(string fileName, int messagesCount)
        {
            var res = await _analysisServiceClient.GetAnalysisResult(fileName);

            Assert.Equal(messagesCount, res.Length);
        }

        [SkippableTheory]
        [InlineData("1.docx.aes", 1)]
        [InlineData("2.docx.aes", 0)]
        [InlineData("3.docx.aes", 0)]
        [InlineData("4.docx.aes", 0)]
        [InlineData("5.docx.aes", 0)]
        [InlineData("6.docx.aes", 0)]
        [InlineData("7.docx.aes", 0)]
        public async void Layout(string fileName, int messagesCount)
        {
            var res = await _analysisServiceClient.GetAnalysisResult(fileName);

            Assert.Equal(messagesCount, res.Length);
        }

        [SkippableTheory]
        [InlineData("1.docx.aes", 10)]
        [InlineData("2.docx.aes", 21)]
        [InlineData("3.docx.aes", 12)]
        [InlineData("4.docx.aes", 2)]
        [InlineData("5.docx.aes", 7)]
        [InlineData("6.docx.aes", 0)]
        [InlineData("7.docx.aes", 1)]
        public async void Orthography2019(string fileName, int messagesCount)
        {
            var res = await _analysisServiceClient.GetAnalysisResult(fileName);

            Assert.Equal(messagesCount, res.Length);
        }

        [SkippableTheory]
        [InlineData("1.docx.aes", 23)]
        [InlineData("2.docx.aes", 18)]
        [InlineData("3.docx.aes", 19)]
        [InlineData("4.docx.aes", 50)]
        [InlineData("5.docx.aes", 55)]
        [InlineData("6.docx.aes", 22)]
        [InlineData("7.docx.aes", 120)]
        public async void Punctuation(string fileName, int messagesCount)
        {
            var res = await _analysisServiceClient.GetAnalysisResult(fileName);

            Assert.Equal(messagesCount, res.Length);
        }

        [SkippableTheory]
        [InlineData("1.docx.aes", 1)]
        [InlineData("2.docx.aes", 0)]
        [InlineData("3.docx.aes", 1)]
        [InlineData("4.docx.aes", 1)]
        [InlineData("5.docx.aes", 0)]
        [InlineData("6.docx.aes", 1)]
        [InlineData("7.docx.aes", 0)]
        public async void References(string fileName, int messagesCount)
        {
            var res = await _analysisServiceClient.GetAnalysisResult(fileName);

            Assert.Equal(messagesCount, res.Length);
        }

        [SkippableTheory]
        [InlineData("1.docx.aes", 1)]
        [InlineData("2.docx.aes", 0)]
        [InlineData("3.docx.aes", 1)]
        [InlineData("4.docx.aes", 5)]
        [InlineData("5.docx.aes", 3)]
        [InlineData("6.docx.aes", 0)]
        [InlineData("7.docx.aes", 20)]
        public async void Runglish(string fileName, int messagesCount)
        {
            var res = await _analysisServiceClient.GetAnalysisResult(fileName);

            Assert.Equal(messagesCount, res.Length);
        }

        [SkippableTheory]
        [InlineData("1.docx.aes", 38)]
        [InlineData("2.docx.aes", 16)]
        [InlineData("3.docx.aes", 32)]
        [InlineData("4.docx.aes", 16)]
        [InlineData("5.docx.aes", 3)]
        [InlineData("6.docx.aes", 2)]
        [InlineData("7.docx.aes", 18)]
        public async void WordingMisuse(string fileName, int messagesCount)
        {
            var res = await _analysisServiceClient.GetAnalysisResult(fileName);

            Assert.Equal(messagesCount, res.Length);
        }
    }
}