
public class OrderItemWithPromotion
{
    public int ProductID { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public decimal Total { get; set; }
    public List<Promotion> AppliedPromotions { get; set; } 
}