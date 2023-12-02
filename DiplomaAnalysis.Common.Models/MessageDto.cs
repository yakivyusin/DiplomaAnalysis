namespace DiplomaAnalysis.Common.Models;

public class MessageDto
{
    public string Code { get; set; }

    public bool IsError { get; set; }

    public string ExtraMessage { get; set; }

    public override string ToString() => $"{Code}: {ExtraMessage}";
}
