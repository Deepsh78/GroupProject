namespace GroupApi.DTOs.Books
{
    public class BookReadDto
    {
        public Guid BookId { get; set; }
        public string BookName { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int Stock { get; set; }
        public Guid PublisherId { get; set; }
        public string PublisherName { get; set; }
        public DateTime PublicationDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsComingSoon { get; set; }
        public string? BookImage { get; set; }


    }

}
