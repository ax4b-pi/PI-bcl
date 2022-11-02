using System.Collections.Generic;
using System.Linq;

namespace PIBcl.Web
{

   public class PaginatedItems<TEntity> where TEntity : class
   {
      public int PageIndex { get; private set; }

      public int PageSize { get; private set; }

      public long Count { get; private set; }

      public bool MoreRecords { get; set; }

      public IEnumerable<TEntity> Data { get; set; }

      public PaginatedItems(int pageIndex, int pageSize, int count, IEnumerable<TEntity> data)
      {
         this.PageIndex = pageIndex;
         this.PageSize = pageSize;
         this.Count = count;
         this.Data = data.Skip(pageSize * pageIndex).Take(pageSize);
         if(PageSize != 0 )
            this.MoreRecords =  Count / PageSize > PageIndex + 1;
      }
   }
}
