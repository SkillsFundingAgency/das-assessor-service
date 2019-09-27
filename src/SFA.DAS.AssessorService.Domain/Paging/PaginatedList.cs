using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.Paging
{
    public class PaginatedList<T>
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int PageSetSize { get; set; }

        public List<T> Items { get; } = new List<T>();

        public int TotalRecordCount { get; }
        public int TotalPages => (TotalRecordCount / PageSize) + ((TotalRecordCount % PageSize) > 0 ? 1 : 0);

        private PaginatedList(List<T> items, int totalRecordCount, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecordCount = totalRecordCount;

            if (items != null)
            {
                Items.AddRange(items);
            }
        }

        public PaginatedList(List<T> items, int totalRecordCount, int pageIndex, int pageSize, int? pageSetSize = null)
            : this(items, totalRecordCount, pageIndex, pageSize)
        {
            PageSetSize = pageSetSize ?? 1;
        }

        public bool HasPreviousPage => (PageIndex > 1);
        public bool HasNextPage => (PageIndex < TotalPages);

        public int FirstVisiblePage => Math.Max(Math.Min(PageIndex - (PageSetSize / 3), (TotalPages + 1) - PageSetSize), 1);
        public int LastVisiblePage => Math.Min(Math.Max(PageIndex + (PageSetSize / 2), PageSetSize), TotalPages);

        public PaginatedList<T1> Convert<T1>() where T1 : class
        {
            return new PaginatedList<T1>(Items.ConvertAll(p => p as T1), TotalRecordCount, PageIndex, PageSize, PageSetSize);
        }
    }
}