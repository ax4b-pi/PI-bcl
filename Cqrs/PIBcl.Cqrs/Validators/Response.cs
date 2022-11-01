using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PIBcl.Cqrs.Validators
{
   public class Response
   {
      private readonly IList<string> _messages = new List<string>();

      public IEnumerable<string> Messages { get; set; }
      public object Result { get; set; }

      public Response() => Messages = new ReadOnlyCollection<string>(_messages);

      public Response(object result) : this() => Result = result;

      public Response AddError(string message)
      {
         _messages.Add(message);
         return this;
      }
   }
}
