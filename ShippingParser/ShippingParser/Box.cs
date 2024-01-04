using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ShippingParser;

public class Box
{
    [Key] 
    public int Id { get; set; }
    public string Identifier { get; set; }
    public string SupplierIdentifier { get; set; }

    public List<Content> Contents { get; set; }

    public class Content
    {
        [Key] 
        public int Id { get; set; }
        public string PoNumber { get; set; }
        public string Isbn { get; set; }
        public int Quantity { get; set; }
    }
}