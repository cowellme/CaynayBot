using CaynayBot.Models;
using CaynayBot.Repositories;

namespace CaynayBot.Services
{
    public interface IProductService
    {
        Task UpdateProduct(Product product);
        Task AddProduct(Product product);
        Task <IEnumerable<Product>> GetAllAsync();
        Task AddProducts(List<Product> resetProducts);
        Task<Product?> GetByIdAsync(int id);
    }
    
    
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productService;

        public ProductService(IRepository<Product> userProduct)
        {
            _productService = userProduct;
        }

        public async Task UpdateProduct(Product product)
        {
            await Task.Run(() => _productService.Update(product));
            await _productService.SaveChangesAsync();
        }
        public async Task AddProduct(Product product)
        {
            await Task.Run(() => _productService.AddAsync(product));
            await _productService.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var result = await _productService.GetAllAsync();
            return [.. result];
        }

        public async Task AddProducts(List<Product> resetProducts)
        {
            await _productService.AddRangeAsync(resetProducts);
            await _productService.SaveChangesAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productService.GetByIdAsync(id);
        }
    }
}