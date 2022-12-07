
namespace PIBcl.Cqrs.Models
{
    public class AssignParamsManipulationEntity
    {
        public string Query { get; set; }
        public object Params { get; set; }

        public AssignParamsManipulationEntity(string query, object @params)
        {
            Query = query;
            Params = @params;
        }
    }
}
