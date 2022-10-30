﻿using System;
using System.Text.RegularExpressions;

namespace DiplomaAnalysis.Common
{
    public static class RegexExtensions
    {
        public static string GetMatchTextWithContext(this Match match, string text, int range)
        {
            var startIndex = Math.Max(0, match.Index - range);
            var length = 2 * range + match.Length;

            if (startIndex + length >= text.Length)
            {
                length = text.Length - startIndex;
            }

            return text
                .Substring(startIndex, length);
        }
    }
}
