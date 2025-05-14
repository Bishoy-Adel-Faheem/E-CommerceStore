using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace EcommerceApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
