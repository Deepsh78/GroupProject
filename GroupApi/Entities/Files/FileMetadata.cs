using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GroupApi.Entities.Files
{
    [Table("FileMetadata")]
    public class FileMetadata
    {
        [Key]
        public Guid FileId { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public long FileSize { get; set; }

        [Required]
        public DateTime UploadedAt { get; set; }
    }
}