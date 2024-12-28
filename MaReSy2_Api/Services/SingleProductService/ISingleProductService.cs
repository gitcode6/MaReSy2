using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace MaReSy2_Api.Services.SingleProductService
{
    public interface ISingleProductService
    {
        Task<APIResponse<IEnumerable<SingleProductDTO>>> GetSingleProductsAsync();

        Task<APIResponse<SingleProductDTO>> GetSingleProductAsync(int productId);

        Task<APIResponse<string>> AddNewSingleProduct(CreateSingleProductDTO singleProduct);

        Task<APIResponse<string>> updateSingleProduct(int productId, UpdateSingleProductDTO product);

        Task<APIResponse<string>> deleteSingleProductAsync(int productId);








        Task<bool> ProductSerialNumberExists(string serialNumber);
        Task<bool> SingleProductExistsAsync(int productId);

        Task<List<SingleProduct>> GetRentedSingleproducts(int productId);

        Task<List<SingleProduct>> GetSingleproductsForProduct(int productId);

        Task<List<SingleProduct>> GetRentableSingleproducts(int productId);

        Task<List<SingleProduct>> GetNeededSingleProducts(int productId, int amount);

        //Task<ProductDTO?> GetProductByIdAsync(int productId);

        //Task<List<IdentityResult>> AddNewSingleProduct(CreateProductDTO product);

        //Task<bool> SingleProductExistsAsync(int productId);
        //Task<bool> SingleProductExistsAsync(string productname);

        //Task<List<IdentityResult>> updateProduct(int id, UpdateProductDTO product);

        //Task<IdentityResult> deleteProductAsync(int productId);
    }
}
