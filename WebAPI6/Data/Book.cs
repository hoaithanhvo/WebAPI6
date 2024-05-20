using System;
using System.Collections.Generic;

namespace WebAPI6.Data
{
    public partial class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
    }
}
