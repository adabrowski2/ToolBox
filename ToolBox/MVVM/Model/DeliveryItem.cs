namespace Praca_Inżynierska_v1.MVVM.Model
{
    public class DeliveryItem
    {
        public int DeliveryItemId { get; set; }
        public int DeliveryID { get; set; }
        public int ProductID { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int ReceivedQuantity { get; set; }
    }
}
