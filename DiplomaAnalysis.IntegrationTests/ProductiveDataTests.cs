using DiplomaAnalysis.IntegrationTests.Infrastructure;

namespace DiplomaAnalysis.IntegrationTests;

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
    [InlineData("1.docx.aes", 27)]
    [InlineData("2.docx.aes", 23)]
    [InlineData("3.docx.aes", 49)]
    [InlineData("4.docx.aes", 50)]
    [InlineData("5.docx.aes", 61)]
    [InlineData("6.docx.aes", 29)]
    [InlineData("7.docx.aes", 142)]
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
    [InlineData("1.docx.aes", 3)]
    [InlineData("2.docx.aes", 1)]
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
    [InlineData("2.docx.aes", 19)]
    [InlineData("3.docx.aes", 33)]
    [InlineData("4.docx.aes", 19)]
    [InlineData("5.docx.aes", 3)]
    [InlineData("6.docx.aes", 8)]
    [InlineData("7.docx.aes", 20)]
    public async void WordingMisuse(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [SkippableTheory]
    [InlineData("1.docx.aes", 0)]
    [InlineData("2.docx.aes", 0)]
    [InlineData("3.docx.aes", 3)]
    [InlineData("4.docx.aes", 0)]
    [InlineData("5.docx.aes", 0)]
    [InlineData("6.docx.aes", 0)]
    [InlineData("7.docx.aes", 2)]
    public async void Pronouns(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [SkippableTheory]
    [InlineData("1.docx.aes", 0)]
    [InlineData("2.docx.aes", 0)]
    [InlineData("3.docx.aes", 0)]
    [InlineData("4.docx.aes", 0)]
    [InlineData("5.docx.aes", 0)]
    [InlineData("6.docx.aes", 0)]
    [InlineData("7.docx.aes", 0)]
    public async void Table(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [SkippableTheory]
    [InlineData("1.docx.aes", 0)]
    [InlineData("2.docx.aes", 0)]
    [InlineData("3.docx.aes", 0)]
    [InlineData("4.docx.aes", 0)]
    [InlineData("5.docx.aes", 0)]
    [InlineData("6.docx.aes", 0)]
    [InlineData("7.docx.aes", 0)]
    public async void Image(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }
}