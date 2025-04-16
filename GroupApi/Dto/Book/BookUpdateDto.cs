namespace GroupApi.Dto.Book
{
    public class BookUpdateDto
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime PublicationDate { get; set; }
        public string Description { get; set; }
    }
}
