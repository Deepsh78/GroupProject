namespace GroupApi.DTOs.Books
{
    public class BookCreateDto
    {
        public string BookName { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public int Stock { get; set; }
        public Guid PublisherId { get; set; }
        public DateTime PublicationDate { get; set; }
        public bool IsComingSoon { get; set; }
        public IFormFile? BookImage { get; set; }
    }

}
