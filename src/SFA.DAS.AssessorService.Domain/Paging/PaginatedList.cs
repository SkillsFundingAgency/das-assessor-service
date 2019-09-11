using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.Paging
{
    public abstract class PaginatedList
    {
        public static int GetFirstPageOfPageSet(int pageSetIndex, int pageSetSize)
        {
            return (pageSetIndex * pageSetSize) - (pageSetSize - 1);
        }
    }

    public class PaginatedList<T> : PaginatedList
    {
        public int PageIndex { get; }
        public int PageSize { get; }
        public int PageSetIndex => (PageIndex / PageSetSize) + ((PageIndex % PageSetSize) > 0 ? 1 : 0);
        public int PageSetSize { get; set; } = 1;

        public List<T> Items { get; } = new List<T>();

        public int TotalRecordCount { get; }
        public int TotalPages => (TotalRecordCount / PageSize) + ((TotalRecordCount % PageSize) > 0 ? 1 : 0);
        public int TotalPageSets => (TotalPages / PageSetSize) + ((TotalPages % PageSetSize) > 0 ? 1 : 0);

        public PaginatedList(List<T> items, int totalRecordCount, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecordCount = totalRecordCount;

            if (items != null)
            {
                Items.AddRange(items);
            }
        }

        public bool HasPreviousPage => (PageSetIndex > 1);
        public bool HasNextPage => (PageSetIndex < TotalPageSets);

        public int? NextPageSetIndex => PageSetIndex + 1;
        public int? PrevPageSetIndex => PageSetIndex - 1 > 0 ? PageSetIndex - 1 : (int?)null;

        public int FirstPageInPageSet => ((PageSetIndex - 1) * PageSetSize) + 1;
        public int LastPageInPageSet => Math.Min(PageSetIndex * PageSetSize, TotalPages);
    }
}