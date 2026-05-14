namespace Praca_Inżynierska_v1.MVVM.Model
{
    public class Product
    {

        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public int? MinimumStock { get; set; }
        public bool? IsActive { get; set; }
        public decimal? Discount { get; set; } // 0 jeśli brak aktywnego rabatu
        public DateTime? DiscountStartDate { get; set; }
        public DateTime? DiscountEndDate { get; set; }
        public Product Clone()
        {
            return new Product
            {
                ProductId = this.ProductId,
                Name = this.Name,
                Price = this.Price,
                Quantity = this.Quantity,
                Unit = this.Unit,
                MinimumStock = this.MinimumStock,
                IsActive = this.IsActive
            };
        }

    }
}
