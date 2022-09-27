using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;

namespace PIBcl.Cqrs.Exceptions
{
   [DebuggerStepThrough]
   public static class ThrowPipelineException
   {
      public static void IdentifiedCommandWithoutId()
      {
         throw new PipelineException(
             "Invalid IdentifiedCommand",
             new ValidationException("Validation Exception",
                 new[] { new ValidationFailure("Id", "Não foi especificado um ID válido para este Comando.") }
             )
         );
      }

      public static void IdentifiedCommandWithoutInnerCommand()
      {
         throw new PipelineException(
             "Invalid IdentifiedCommand",
             new ValidationException(
                 "Validation Exception",
                 new[] { new ValidationFailure("Command", "Não foi especificado um Comando válido para ser executado.") }
             )
         );
      }

      public static void CommandWasAlreadyExecuted()
      {
         throw new PipelineException(
             "Invalid IdentifiedCommand",
             new ValidationException("Validation Exception",
                 new[]
                 {
                        new ValidationFailure("Id", "Este Comando já foi executado.")
                 }));
      }
   }
}
