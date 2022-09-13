using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PIBcl.Core.Extensions
{
    public static class DoubleExtensions
    {
        public static string ToRealBrasil(this double s)
        {
            return s.ToString("C", CultureInfo.CreateSpecificCulture("pt-BR"));
        }

        public static string ToPercentualBrasil(this double s)
        {
            return s.ToString("P", CultureInfo.CreateSpecificCulture("pt-BR"));
        }
    }
}
