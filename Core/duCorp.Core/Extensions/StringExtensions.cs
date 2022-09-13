using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DuCorp.Core.Extensions
{
    public static class StringExtensions
    {

        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";

            TextInfo cultInfo = new CultureInfo("en-US", false).TextInfo;
            str = cultInfo.ToTitleCase(str);
            str = str.Replace(" ", "");
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string RemoveAccents(this string input)
        {
            string normalized = input.Normalize(NormalizationForm.FormKD);
            Encoding removal = Encoding.GetEncoding(Encoding.ASCII.CodePage,
                                                    new EncoderReplacementFallback(""),
                                                    new DecoderReplacementFallback(""));
            byte[] bytes = removal.GetBytes(normalized);
            return Encoding.ASCII.GetString(bytes);
        }


    }
}
