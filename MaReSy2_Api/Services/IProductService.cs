using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.RentalDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace MaReSy2_Api.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProductsAsync();

        Task<ProductDTO?> GetProductByIdAsync(int productId);

        Task<List<IdentityResult>> AddNewProduct(CreateProductDTO product);

        Task<bool> ProductExistsAsync(int productId);
        Task<bool> ProductExistsAsync(string productname);

        Task<List<IdentityResult>> updateProduct(int id, UpdateProductDTO product);

        Task<IdentityResult> deleteProductAsync(int productId);

        Task<List<ProductWithAmount>> GetProductsForSet(int setId);


    }
}
