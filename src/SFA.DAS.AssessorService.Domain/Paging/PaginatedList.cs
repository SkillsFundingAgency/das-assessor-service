using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.Domain.Paging
{
    public class PaginatedList<T> 
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }
 
        public List<T> Items { get; } = new List<T>();       
 
        public int TotalPages { get; set; }
 
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecordCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
 
            Items.AddRange(items);
        }
 
        public bool HasPreviousPage => (PageIndex > 0);
        public bool HasNextPage => (PageIndex < TotalPages - 1);
    }
}