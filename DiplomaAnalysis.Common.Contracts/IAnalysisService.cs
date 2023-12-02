using DiplomaAnalysis.Common.Models;
using System;
using System.Collections.Generic;

namespace DiplomaAnalysis.Common.Contracts;

public interface IAnalysisService : IDisposable
{
    IReadOnlyCollection<MessageDto> Analyze();
}
