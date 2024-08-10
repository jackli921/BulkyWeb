using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Bulky.Models;

public class OrderHeader
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    [ForeignKey("ApplicationUserId")]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; }

    public DateTime OrderDate { get; set; }
    public DateTime ShippingDate { get; set; }
    public double OrderTotal { get; set; }

    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }

    public DateTime PaymentDate { get; set; }
    public DateOnly PaymentDueDate { get; set; }
    public string? SessionId { get; set; }
    public string? PaymentIntentId { get; set; }
    
    [Required]
    [Phone]
    [StringLength(15, MinimumLength = 10)]
    public string PhoneNumber { get; set; }

    [Required]
    [StringLength(100)]
    public string StreetAddress { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The city name can only contain letters and spaces.")]
    [StringLength(50)]
    public string City { get; set; }

    [Required]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The state name can only contain letters and spaces.")]
    [StringLength(2, MinimumLength = 2)]
    public string State { get; set; }

    [Required]
    [RegularExpression(@"^\d{5}(-\d{4})?$", ErrorMessage = "Invalid postal code.")]
    [StringLength(10)]
    public string PostalCode { get; set; }
    
    [Required]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The name can only contain letters and spaces.")]
    [StringLength(50)]
    public string Name { get; set; }

}