using DiplomaAnalysis.IntegrationTests.Infrastructure;

namespace DiplomaAnalysis.IntegrationTests;

public class SyntheticDataTests
{
    private static readonly AnalysisServiceClient _analysisServiceClient = new()
    {
        FileProvider = new(decryptFiles: false)
    };

    [Test]
    [Arguments("1.docx", 1)]
    public async Task CharReplacement(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("1.docx", 1)]
    public async Task Layout(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("1.docx", 3)]
    public async Task Orthography2019(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("1.docx", 1)]
    public async Task Punctuation(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("1.docx", 1)]
    [Arguments("4.docx", 0)]
    [Arguments("5.docx", 1)]
    public async Task References(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("1.docx", 28)]
    public async Task Runglish(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("1.docx", 6)]
    public async Task WordingMisuse(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("1.docx", 9)]
    public async Task Pronouns(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("2.docx", 6)]
    public async Task Table(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [Arguments("3.docx", 8)]
    public async Task Image(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }
}