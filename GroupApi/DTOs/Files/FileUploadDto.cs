using System.ComponentModel.DataAnnotations;

namespace GroupApi.DTOs.Files
{
   
        public class FileUploadDto
        {
            public IFormFile File { get; set; }
        }
    
}