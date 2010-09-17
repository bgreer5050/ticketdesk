//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System; 
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TicketDesk.Domain.Models.DataAnnotations
{
    public partial class TicketAttachmentMeta
    {
    		
    	[DisplayName("Ticket Id")]
        public Nullable<int> TicketId { get; set; }
    		
    	[DisplayName("File Id")]
    	[Required]
        public int FileId { get; set; }
    		
    	[DisplayName("File Name")]
    	[Required]
    	[StringLength(255)]
        public string FileName { get; set; }
    		
    	[DisplayName("File Size")]
    	[Required]
        public int FileSize { get; set; }
    		
    	[DisplayName("File Type")]
    	[Required]
    	[StringLength(250)]
        public string FileType { get; set; }
    		
    	[DisplayName("Uploaded By")]
    	[Required]
    	[StringLength(100)]
        public string UploadedBy { get; set; }
    		
    	[DisplayName("Uploaded Date")]
    	[Required]
        public System.DateTime UploadedDate { get; set; }
    		
    	[DisplayName("File Contents")]
    	[Required]
        public byte[] FileContents { get; set; }
    		
    	[DisplayName("File Description")]
    	[StringLength(500)]
        public string FileDescription { get; set; }
    		
    	[DisplayName("Is Pending")]
    	[Required]
        public bool IsPending { get; set; }
    }
}
