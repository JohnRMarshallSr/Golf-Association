//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GA.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Documents
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Documents()
        {
            this.EventDocuments = new HashSet<EventDocuments>();
        }
    
        public int DocumentId { get; set; }
        public byte[] DocumentContent { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EventDocuments> EventDocuments { get; set; }
    }
}