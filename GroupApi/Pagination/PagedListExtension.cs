namespace GroupApi.Pagination
{
    public static class PagedListExtension
    {
        public static async Task<PaginatedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize)
        {
            return await new PaginatedList<T>().CreateAsync(source, pageNumber, pageSize);
        }
    }
}
