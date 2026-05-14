namespace Praca_Inżynierska_v1.MVVM.Model
{
    public class Delivery
    {
        public int DeliveryId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public int ItemCount { get; set; }
    }
}
