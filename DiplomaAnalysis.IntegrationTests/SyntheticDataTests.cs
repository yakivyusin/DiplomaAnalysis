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
    [InlineData("1.docx", 1)]
    public async void Punctuation(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("1.docx", 1)]
    [InlineData("4.docx", 0)]
    [InlineData("5.docx", 1)]
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

    [Theory]
    [InlineData("1.docx", 6)]
    public async void Pronouns(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("2.docx", 5)]
    public async void Table(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }

    [Theory]
    [InlineData("3.docx", 7)]
    public async void Image(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        Assert.Equal(messagesCount, res.Length);
    }
}