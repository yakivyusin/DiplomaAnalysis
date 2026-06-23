using DiplomaAnalysis.IntegrationTests.Infrastructure;

namespace DiplomaAnalysis.IntegrationTests;

public class ProductiveDataTests
{
    private static readonly AnalysisServiceClient _analysisServiceClient = new()
    {
        FileProvider = new(decryptFiles: true)
    };

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 0)]
    [Arguments("2.docx.aes", 0)]
    [Arguments("3.docx.aes", 2)]
    [Arguments("4.docx.aes", 0)]
    [Arguments("5.docx.aes", 1)]
    [Arguments("6.docx.aes", 0)]
    [Arguments("7.docx.aes", 0)]
    [Arguments("8.docx.aes", 0)]
    [Arguments("9.docx.aes", 0)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task CharReplacement(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 1)]
    [Arguments("2.docx.aes", 0)]
    [Arguments("3.docx.aes", 0)]
    [Arguments("4.docx.aes", 0)]
    [Arguments("5.docx.aes", 0)]
    [Arguments("6.docx.aes", 0)]
    [Arguments("7.docx.aes", 0)]
    [Arguments("8.docx.aes", 0)]
    [Arguments("9.docx.aes", 0)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task Layout(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 10)]
    [Arguments("2.docx.aes", 21)]
    [Arguments("3.docx.aes", 12)]
    [Arguments("4.docx.aes", 2)]
    [Arguments("5.docx.aes", 7)]
    [Arguments("6.docx.aes", 0)]
    [Arguments("7.docx.aes", 1)]
    [Arguments("8.docx.aes", 0)]
    [Arguments("9.docx.aes", 0)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task Orthography2019(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 30)]
    [Arguments("2.docx.aes", 31)]
    [Arguments("3.docx.aes", 53)]
    [Arguments("4.docx.aes", 59)]
    [Arguments("5.docx.aes", 48)]
    [Arguments("6.docx.aes", 46)]
    [Arguments("7.docx.aes", 73)]
    [Arguments("8.docx.aes", 10)]
    [Arguments("9.docx.aes", 30)]
    [Arguments("10.docx.aes", 9)]
    [Arguments("11.docx.aes", 23)]
    public async Task Punctuation(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 1)]
    [Arguments("2.docx.aes", 0)]
    [Arguments("3.docx.aes", 1)]
    [Arguments("4.docx.aes", 1)]
    [Arguments("5.docx.aes", 0)]
    [Arguments("6.docx.aes", 1)]
    [Arguments("7.docx.aes", 0)]
    [Arguments("8.docx.aes", 0)]
    [Arguments("9.docx.aes", 0)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task References(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 3)]
    [Arguments("2.docx.aes", 1)]
    [Arguments("3.docx.aes", 1)]
    [Arguments("4.docx.aes", 9)]
    [Arguments("5.docx.aes", 3)]
    [Arguments("6.docx.aes", 1)]
    [Arguments("7.docx.aes", 20)]
    [Arguments("8.docx.aes", 1)]
    [Arguments("9.docx.aes", 1)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task Runglish(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 38)]
    [Arguments("2.docx.aes", 19)]
    [Arguments("3.docx.aes", 34)]
    [Arguments("4.docx.aes", 20)]
    [Arguments("5.docx.aes", 4)]
    [Arguments("6.docx.aes", 9)]
    [Arguments("7.docx.aes", 20)]
    [Arguments("8.docx.aes", 1)]
    [Arguments("9.docx.aes", 3)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task WordingMisuse(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 0)]
    [Arguments("2.docx.aes", 0)]
    [Arguments("3.docx.aes", 3)]
    [Arguments("4.docx.aes", 0)]
    [Arguments("5.docx.aes", 0)]
    [Arguments("6.docx.aes", 0)]
    [Arguments("7.docx.aes", 2)]
    [Arguments("8.docx.aes", 0)]
    [Arguments("9.docx.aes", 1)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task Pronouns(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 0)]
    [Arguments("2.docx.aes", 0)]
    [Arguments("3.docx.aes", 0)]
    [Arguments("4.docx.aes", 0)]
    [Arguments("5.docx.aes", 0)]
    [Arguments("6.docx.aes", 0)]
    [Arguments("7.docx.aes", 0)]
    [Arguments("8.docx.aes", 0)]
    [Arguments("9.docx.aes", 1)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task Table(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }

    [Test]
    [SkipProductiveData]
    [Arguments("1.docx.aes", 0)]
    [Arguments("2.docx.aes", 0)]
    [Arguments("3.docx.aes", 0)]
    [Arguments("4.docx.aes", 0)]
    [Arguments("5.docx.aes", 0)]
    [Arguments("6.docx.aes", 0)]
    [Arguments("7.docx.aes", 0)]
    [Arguments("8.docx.aes", 0)]
    [Arguments("9.docx.aes", 2)]
    [Arguments("10.docx.aes", 0)]
    [Arguments("11.docx.aes", 0)]
    public async Task Image(string fileName, int messagesCount)
    {
        var res = await _analysisServiceClient.GetAnalysisResult(fileName);

        await Assert.That(res.Length).IsEqualTo(messagesCount);
    }
}