using System;

namespace Combiner
{
    public static class StringExtensions
    {
        public static string TrimEnd(this string input, string end)
        {
            if (input.EndsWith(end, StringComparison.OrdinalIgnoreCase))
            {
                return input.Substring(0, input.Length - end.Length);
            }

            return input;
        }
    }
}
