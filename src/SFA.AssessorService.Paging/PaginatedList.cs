using System;
using System.Collections.Generic;

namespace SFA.AssessorService.Paging
{
    public class PaginatedList<T> // : List<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; set; }

        public List<T> Items { get; } = new List<T>();       

        private int TotalPages { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalRecordCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            Items.AddRange(items);
        }

        public bool HasPreviousPage => (PageIndex > 1);
        public bool HasNextPage => (PageIndex < TotalPages);

        //public static async Task<PaginatedList<T>> CreateAsync(
        //    IQueryable<T> source, int pageIndex, int pageSize)
        //{
        //    var count = await source.CountAsync();
        //    var items = await source.Skip(
        //            (pageIndex - 1) * pageSize)
        //        .Take(pageSize).ToListAsync();
        //    return new PaginatedList<T>(items, count, pageIndex, pageSize);
        //}
    }
}