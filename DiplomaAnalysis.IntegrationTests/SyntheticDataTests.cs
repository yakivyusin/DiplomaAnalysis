using DiplomaAnalysis.IntegrationTests.Infrastructure;

namespace DiplomaAnalysis.IntegrationTests;

public class SyntheticDataTests
{
    private static readonly AnalysisServiceClient _analysisServiceClient = new()
    {
        FileProvider = new(decryptFiles: false)
    };

    [Theory]
    [InlineData("1.docx", 1)]
    public async void CharReplacement(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("1.docx", 1)]
    public async void Layout(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("1.docx", 3)]
    public async void Orthography2019(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("1.docx", 0)]
    public async void Punctuation(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("1.docx", 1)]
    public async void References(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("1.docx", 11)]
    public async void Runglish(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("1.docx", 4)]
    public async void WordingMisuse(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }
}