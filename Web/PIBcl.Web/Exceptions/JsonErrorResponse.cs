using System;
using System.Collections.Generic;
using System.Text;

namespace DuCorp.Web.Exceptions
{
   public class JsonErrorResponse
   {
      public string[] Messages { get; set; }

      public object DeveloperMessage { get; set; }
   }
}
