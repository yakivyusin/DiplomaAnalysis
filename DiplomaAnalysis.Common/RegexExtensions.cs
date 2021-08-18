using System;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Common
{
    public static class RegexExtensions
    {
        public static string GetMatchTextWithContext(this Match match, string text, int range)
        {
            var startIndex = Math.Max(0, match.Index - range);
            var length = Math.Min(
                text.Length - startIndex,
                (match.Index - startIndex) + match.Value.Length + (match.Index - startIndex));

            return text
                .Substring(startIndex, length);
        }
    }
}
