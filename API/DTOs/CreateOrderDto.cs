using System.ComponentModel.DataAnnotations;
using Core.Entities.OrderAggregate;

namespace API.DTOs;

public class CreateOrderDto
{
    [Required]
    public string CartId { get; set; } = string.Empty;
    [Required]
    public int DeliveryMethodId { get; set; }

    // Better create full Dto for these props (ShippingAddress and PaymentSummary)
    // but, this is concept...

    [Required]
    public ShippingAddress ShippingAddress { get; set; } = null!;
    [Required]
    public PaymentSummary PaymentSummary { get; set; } = null!;
}
