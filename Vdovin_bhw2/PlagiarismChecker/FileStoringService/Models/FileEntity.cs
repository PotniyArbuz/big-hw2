using System;
using System.ComponentModel.DataAnnotations;

namespace FileStoringService.Models
{
    public class FileEntity
    {
        [Key]
        public Guid FileId { get; set; }
        public DateTime UploadTimestamp { get; set; }
        public string Content { get; set; }
    }
}