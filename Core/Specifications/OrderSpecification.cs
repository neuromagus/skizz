using Core.Entities.OrderAggregate;

namespace Core.Specifications;

public class OrderSpecification : BaseSpecification<Order>
{
    public OrderSpecification(string email) : base(x => x.BuyerEmail == email)
    {
        AddInclude(x => x.OrderItems);
        AddInclude(x => x.DeliveryMethod);
        AddOrderByDescending(x => x.OrderDate);
    }

    public OrderSpecification(string email, int id) : base(x => x.BuyerEmail == email && x.Id == id)
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }

     // Why wildcard? because I'm lazy dude
    // and did not like create a new class for this
    public OrderSpecification(string paymentIntentId, bool _)
        : base(x => x.PaymentIntentId == paymentIntentId)    
    {
        AddInclude("OrderItems");
        AddInclude("DeliveryMethod");
    }
}
