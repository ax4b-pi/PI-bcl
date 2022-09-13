using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PIBcl.Core.Extensions
{
    public static class DecimalExtensions
    {
        public static string ToRealBrasil(this decimal s)
        {
            return s.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        public static string ToPercentualBrasil(this decimal s)
        {
            return s.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
        }
    }
}
