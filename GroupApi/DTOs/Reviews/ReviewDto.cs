namespace GroupApi.DTOs.Reviews
{
    public class ReviewDto
    {
        public Guid ReviewId { get; set; }
        public Guid BookId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public Guid MemberId { get; set; }

    }

}
