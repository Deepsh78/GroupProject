using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Files
{
    public class FileUploadDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}