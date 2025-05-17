using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EcommerceApp.Models;

namespace EcommerceApp.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }

    public class EditProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class AddressViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [Display(Name = "Address Line")]
        public string Address { get; set; } = string.Empty;
        
        [Required]
        public string City { get; set; } = string.Empty;
        
        [Required]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        public bool IsDefault { get; set; }
        
        public bool IsBilling { get; set; }
    }    public class OrderDetailsViewModel
    {
        public Order Order { get; set; } = null!;
        public List<OrderNote> OrderNotes { get; set; } = new List<OrderNote>();
        public string OrderStatusClass
        {
            get
            {
                return Order.Status switch
                {
                    OrderStatus.Pending => "bg-warning text-dark",
                    OrderStatus.Processing => "bg-info text-dark",
                    OrderStatus.Shipped => "bg-primary",
                    OrderStatus.OutForDelivery => "bg-info",
                    OrderStatus.Delivered => "bg-success",
                    OrderStatus.Cancelled => "bg-danger",
                    OrderStatus.ReturnRequested => "bg-warning text-dark",
                    OrderStatus.Returned => "bg-secondary",
                    _ => "bg-secondary"
                };
            }
        }
        
        public string OrderStatusText
        {
            get
            {
                return Order.Status.ToString();
            }
        }
    }
}
