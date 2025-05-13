namespace GroupApi.DTOs.Reviews
{
    public class UpdateReviewDto
    {
      
            public Guid BookId { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
            

        }
    
}
