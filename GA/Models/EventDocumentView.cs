using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GA.Models
{
    public class EventDocumentView
    {
        public int EventDocumentId { get; set; }
        public int DocumentId { get; set; }
        public string Title { get; set; }
        public string Filename { get; set; }
    }
}