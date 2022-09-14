using System;
using System.Collections.Generic;
using System.Text;

namespace PIBcl.Cqrs.Exceptions
{
   public class ResourceNotFoundException : Exception
   {
      public ResourceNotFoundException(string message, Exception innerException=null)
          : base(message, innerException)
      {
      }
   }
}
