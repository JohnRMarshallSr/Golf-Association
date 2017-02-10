using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GA.Models
{
    [Table("ImageFile")]
    public class ImageFile
    {
        public Guid ImageFileId { get; set; }
        public string Filename { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public long Size { get; set; }
    }
}