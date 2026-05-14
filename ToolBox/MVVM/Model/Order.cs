using Praca_Inżynierska_v1.MVVM.Model;

public class Order
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; }
    public int ClientId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }

    public string ClientName { get; set; }
    public string UserName { get; set; }

    public List<OrderItem> Items { get; set; } = new();

    public decimal TotalPrice => Items.Sum(i =>
        i.Quantity * i.UnitPrice * (1 - (i.Discount ?? 0)));
}
