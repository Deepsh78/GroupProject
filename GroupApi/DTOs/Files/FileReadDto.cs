using System;

namespace GroupApi.DTOs.Files
{
    public class FileReadDto
    {
        public Guid FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}