namespace GroupApi.DTOs.Books
{
    public class BookDetailDto
    {
        public Guid BookId { get; set; }
        public string BookName { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int Stock { get; set; }
        public string PublisherName { get; set; }
        public string? BookImage { get; set; }
        public List<string> Authors { get; set; }
        public List<string> Genres { get; set; }
        public List<string> Formats { get; set; }
        public List<string> Categories { get; set; }
    }

}
