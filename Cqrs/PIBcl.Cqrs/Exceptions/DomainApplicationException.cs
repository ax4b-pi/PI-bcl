using System;
using System.Collections.Generic;
using System.Text;

namespace PIBcl.Cqrs.Exceptions
{
   public class DomainApplicationException : Exception
   {
      public DomainApplicationException(string message, Exception innerException)
          : base(message, innerException)
      {
      }
   }
}
