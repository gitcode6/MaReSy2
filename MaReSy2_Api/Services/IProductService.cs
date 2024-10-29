using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync();

        Task<ProductDTO?> GetProductByIdAsync(int productId);

        Task<(ProductDTO? CreatedProduct, List<string>? errors)> AddNewProduct(string Productname, string Productdescription, bool Productactive, int Productamount);

         Task<bool> ProductExistsAsync(int productId);


    }
}
