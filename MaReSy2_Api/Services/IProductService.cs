using MaReSy2_Api.Models.DTO.ProductDTO;
using Microsoft.AspNetCore.Identity;

namespace MaReSy2_Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync();

        Task<ProductDTO?> GetProductByIdAsync(int productId);

        Task<(ProductDTO? CreatedProduct, List<string>? errors)> AddNewProduct(CreateProductDTO product);

         Task<bool> ProductExistsAsync(int productId);


    }
}
