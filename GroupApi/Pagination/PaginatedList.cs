using Microsoft.EntityFrameworkCore;

namespace GroupApi.Pagination
{
    public class PaginatedList<T>
    {
        public int TotalItems { get; set; }
        public int SearchItems { get; set; }
        public IReadOnlyCollection<T> Items { get; set; }

        public PaginatedList() { }

        public PaginatedList(IReadOnlyCollection<T> items, int totalItems, int searchItems)
        {
            TotalItems = totalItems;
            Items = items;
            SearchItems = searchItems;
        }

        public async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
        {
            var result = await source.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToListAsync();
            var count = await source.CountAsync();
            return new PaginatedList<T>(result, count, result.Count);
        }
    }
}
