using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.Entities
{
    public class Supplier
    {
        [Key] public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}