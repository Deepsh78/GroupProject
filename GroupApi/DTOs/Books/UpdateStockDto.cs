// GroupApi.DTOs/Books/UpdateStockDto.cs
using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Books
{
    public class UpdateStockDto
    {
        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}