namespace GroupApi.DTOs.Books
{
    public class BookFilterDto
    {
        public string? SearchTerm { get; set; }      

        public bool SortByPriceDescending { get; set; }   

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

}
