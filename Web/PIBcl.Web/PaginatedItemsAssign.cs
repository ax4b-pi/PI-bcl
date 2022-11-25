using System.Collections.Generic;

namespace PIBcl.Web
{
    public class PaginatedItemsAssign<TEntity> where TEntity : class
    {
        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public long Count { get; private set; }

        public bool MoreRecords { get; set; }

        public IEnumerable<TEntity> Data { get; set; }

        public PaginatedItemsAssign(int pageIndex, int pageSize, long count,
            IEnumerable<TEntity> data, bool moreRecords)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Count = count;
            this.Data = data;
            this.MoreRecords = moreRecords;
        }
    }
}
