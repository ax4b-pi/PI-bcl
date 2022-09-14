using System;

namespace PIBcl.Cqrs.Exceptions
{
   public class PipelineException : Exception
   {
      public PipelineException(string message, Exception innerException)
          : base(message, innerException)
      {
      }
   }
}
