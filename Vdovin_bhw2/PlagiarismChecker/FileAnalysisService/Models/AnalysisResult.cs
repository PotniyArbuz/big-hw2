using System;
using System.ComponentModel.DataAnnotations;

namespace FileAnalysisService.Models
{
    public class AnalysisResult
    {
        [Key]
        public Guid FileId { get; set; }
        public DateTime UploadTimestamp { get; set; }
        public int Paragraphs { get; set; }
        public int Words { get; set; }
        public int Characters { get; set; }
        public string Hash { get; set; }
        public bool IsPlagiarized { get; set; }
    }
}