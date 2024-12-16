using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace MaReSy2_Api.Services
{
    public interface ISingleProductService
    {
        Task<IEnumerable<SingleProductDTO>> GetSingleProductsAsync();

        Task<SingleProductDTO?> GetSingleProductAsync(int productId);

        Task<List<IdentityResult>> AddNewSingleProduct(CreateSingleProductDTO singleProduct);

        Task<bool> ProductSerialNumberExists(string serialNumber);
        Task<bool> SingleProductExistsAsync(int productId);

        Task<List<IdentityResult>> updateSingleProduct(int productId, UpdateSingleProductDTO product);

        Task<IdentityResult> deleteSingleProductAsync(int productId);

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
