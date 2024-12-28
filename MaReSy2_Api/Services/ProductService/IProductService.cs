using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.RentalDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace MaReSy2_Api.Services.ProductService
{
    public interface IProductService
    {
        //externe Methoden
        Task<APIResponse<IEnumerable<ProductDTO>>> GetProductsAsync();

        Task<APIResponse<ProductDTO>> GetProductByIdAsync(int productId);

        Task<APIResponse<bool>> AddNewProduct(CreateProductDTO product);

        Task<APIResponse<bool>> updateProduct(int id, UpdateProductDTO product);

        Task<APIResponse<bool>> deleteProductAsync(int productId);




        //interne Methoden
        Task<List<ProductWithAmount>> GetProductsForSet(int setId);
        Task<bool> SetContainsInactiveProduct(int setId);
        Task<bool> ProductExistsAsync(int productId);
        Task<bool> ProductExistsAsync(string productname);



    }
}
