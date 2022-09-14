using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerBaseExtensions
    {
        public static bool ArrayIsValid(this ControllerBase c, 
            string values, out string[] validateValues)
        {
            validateValues = null;

            if (string.IsNullOrEmpty(values))
                return true;

            values = values.Replace(" ", string.Empty);
            var numIds = values.Split(',').Select(value => (Ok: true, Value: value));

            if (!numIds.All(nid => nid.Ok))
            {
                return false;
            }

            validateValues = numIds.Select(validateValue => (validateValue.Value)).ToArray();
           
            return true;
        }
    }
}
