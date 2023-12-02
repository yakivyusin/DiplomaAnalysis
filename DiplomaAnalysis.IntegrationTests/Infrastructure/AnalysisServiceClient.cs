using DiplomaAnalysis.Common.Models;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace DiplomaAnalysis.IntegrationTests.Infrastructure;

public class AnalysisServiceClient
{
    private static readonly HttpClient _httpClient = new();

    public TestFileProvider FileProvider { get; set; }

    public async Task<MessageDto[]> GetAnalysisResult(string fileName, [CallerMemberName] string serviceName = null)
    {
        using var payload = GetAnalysisPayload(fileName);
        var apiPath = $"{EnvironmentVariables.ApplicationPath}/api/{serviceName}";

        var response = await _httpClient.PostAsync(apiPath, payload);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MessageDto[]>();
    }

    private MultipartFormDataContent GetAnalysisPayload(string fileName)
    {
        var multipartFormContent = new MultipartFormDataContent();
        var content = new ByteArrayContent(FileProvider.GetFile($"TestFiles\\{fileName}"));
        content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");

        multipartFormContent.Add(content, name: "file", fileName: fileName);

        return multipartFormContent;
    }
}
