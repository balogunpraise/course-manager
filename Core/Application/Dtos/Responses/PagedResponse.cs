using Microsoft.EntityFrameworkCore;

namespace Core.Application.Dtos.Responses
{
    public class PagedList<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PageSize { get; set; } = 20;
        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;



        public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageSize, int currentPage, int totalCount)
        {
            var pagedList = new PagedList<T>
            {
                Items = await source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync(),
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalCount = totalCount,
                TotalPages = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1,
            };
            return pagedList;
        }


        public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageSize, int currentPage, int totalCount)
        {
            return new PagedList<T>
            {
                Items = source.ToList(),
                PageSize = pageSize,
                CurrentPage = currentPage,
                TotalCount = totalCount,
                TotalPages = totalCount % pageSize == 0 ? totalCount / pageSize : totalCount / pageSize + 1,
            };
        }
    }
}
