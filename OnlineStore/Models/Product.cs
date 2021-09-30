using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OnlineStore.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Details { get; set; }
        [Required]
        public string img { get; set; }
        [Required]
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Rating { get; set; }
        [Required]
        public int Quantity { get; set; }
        public bool IsNewArrival { get; set; }
        public bool IsFreeDelivery { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsBestSeller { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool OutOfStock { get; set; }

        public virtual Category Category { get; set; }
        public ICollection<Reviews> Reviews { get; set; }
    }

    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Details { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool OutOfStock { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Order Date")]
        public DateTime OrderDate { get; set; }
        [Display(Name = "Customer Name")]
        public string FullName { get; set; }
        [Display(Name = "Phone Number")]
        public string MobileNumber { get; set; }
        public string City { get; set; }
        public string Address { get; set; }

        public bool IsActive { get; set; }
        public bool Deliverd { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }
        public virtual Order order { get; set; }
        public virtual Product Product { get; set; }
    }

    public class Shippings
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Landmark { get; set; }
        public string MobileNumber { get; set; }
        public string City { get; set; }
        public string PaymentMethod { get; set; }
        public decimal AmountPaid { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Order Order { get; set; }
    }

    public class Reviews
    {
        public int Id { get; set; }
        public decimal Rate { get; set; }
        public string Comment { get; set; }

        public ApplicationUser user { get; set; }
        public int ProductId { get; set; }
    }
}