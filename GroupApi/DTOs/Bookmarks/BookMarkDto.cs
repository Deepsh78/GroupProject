namespace GroupApi.DTOs.Bookmarks
{
    public class BookMarkDto
    {
        public Guid BookMarkId { get; set; } // Primary Key
        public Guid BookId { get; set; } // Foreign Key for Book
        public Guid MemberId { get; set; } // Foreign Key for Member
        public string BookName { get; set; } // Book name (for the read DTO)
    }

}
