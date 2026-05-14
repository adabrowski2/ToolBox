using Praca_Inżynierska_v1.MVVM.Model;


namespace Praca_Inżynierska_v1.Services.Product_service
{
    internal interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task UpdateProductsAsync(IEnumerable<Product> products);
        Task DecreaseQuantityAsync(int productId, decimal quantity);
        Task UpdateProductQuantityAsync(int productId, decimal quantity);
    }
}
