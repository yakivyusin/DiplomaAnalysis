using System;

namespace DiplomaAnalysis.Common.Extensions;

public static class StringExtensions
{
    public static string TakeFirst(this string str, int count) => str == null ? string.Empty : str[0..Math.Min(count, str.Length)];
    
    public static string TakeLast(this string str, int count) => str == null ? string.Empty : str[Math.Max(0, str.Length - count)..];
}
