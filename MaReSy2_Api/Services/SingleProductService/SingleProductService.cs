using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using MaReSy2_Api.Services.ProductService;
using MaReSy2_Api.Services.SingleProductService;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Transactions;

namespace MaReSy2_Api.Services.SingleProductService
{
    public class SingleProductService : ISingleProductService
    {
        private readonly MaReSyDbContext _context;
        private readonly IProductService _productService;


        public SingleProductService(MaReSyDbContext dbContext, IProductService productService)
        {
            _context = dbContext;
            _productService = productService;
        }

        public async Task<APIResponse<string>> AddNewSingleProduct(CreateSingleProductDTO singleProduct)
        {
            var result = new APIResponse<string>();

            if (string.IsNullOrWhiteSpace(singleProduct.SingleProductName))
            {
                result.Errors.Add(new ErrorDetail { Field = "SingleProductName", Error = "Bitte gib einen Produktnamen an!" });
            }

            if (string.IsNullOrWhiteSpace(singleProduct.SingleProductSerialNumber))
            {
                result.Errors.Add(new ErrorDetail { Field = "SingleProductSerialNumber", Error = "Bitte gib eine Produkt-Serien-Nr. ein!" });
            }

            if (!await _productService.ProductExistsAsync(singleProduct.ProductId))
            {
                result.Errors.Add(new ErrorDetail { Field = "ProductId", Error = "Das Produkt mit dieser Id gibt es nicht!" });
            }

            if (await ProductSerialNumberExists(singleProduct.SingleProductSerialNumber.ToLower().Trim()))
            {
                result.Errors.Add(new ErrorDetail { Field = "SingleProductSerialNumber", Error = "Die Produkt-Serien-Nr. existiert bereits!" });
            }

            if (result.Errors.Any())
            {
                result.StatusCode = 400;
                return result;
            }

            var newSingleProduct = new SingleProduct
            {
                SingleProductName = singleProduct.SingleProductName,
                SingleProductNumber = singleProduct.SingleProductSerialNumber,
                SingleProductActive = singleProduct.SingleProductActive,
                ProductId = singleProduct.ProductId,
            };

            await _context.SingleProducts.AddAsync(newSingleProduct);
            await _context.SaveChangesAsync();

            result.Data = "SingleProduct erfolgreich hinzugefügt!";
            result.StatusCode = 200;
            return result;




        }

        public async Task<APIResponse<string>> deleteSingleProductAsync(int productId)
        {
            var result = new APIResponse<string>();

            var singleProduct = await _context.SingleProducts.FindAsync(productId);


            if (singleProduct == null)
            {
                result.Errors.Add(new ErrorDetail { Field = "ProductId", Error = "SingleProduct wurde nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }

            singleProduct.SingleProductActive = false;

            _context.SingleProducts.Update(singleProduct);
            await _context.SaveChangesAsync();


            result.Data = "SingleProduct erfolgreich deaktiviert!";
            result.StatusCode = 200;
            return result;
        }

        public async Task<APIResponse<SingleProductDTO>> GetSingleProductAsync(int productId)
        {
            var result = new APIResponse<SingleProductDTO>();

            var response = await SingleProductExistsAsync(productId);


            if(response == false)
            {
                result.Errors.Add(new ErrorDetail { Field = "ProductId", Error = "SingleProduct nicht gefunden!" });
                result.StatusCode = 404;
                return result;
            }

            else
            {
                var product = await _context.SingleProducts.Include(x => x.Product).FirstAsync(x => x.SingleProductId == productId);
                result.Data = new SingleProductDTO
                {
                    SingleProductId = product.SingleProductId,
                    SingleProductName = product.SingleProductName,
                    SingleProductActive = product.SingleProductActive,
                    SingleProductSerialNumber = product.SingleProductNumber,
                    MainProduct = product.Product.Productname
                };


                result.StatusCode = 200;

            }

            return result;
        }

        public async Task<APIResponse<IEnumerable<SingleProductDTO>>> GetSingleProductsAsync()
        {
            var response = new APIResponse<IEnumerable<SingleProductDTO>>();


            var SingleProducts = _context.SingleProducts
                .Include(sp => sp.Product);

            var SingleProductList = await SingleProducts.ToListAsync();

            response.Data =  SingleProductList.Select(singleProduct => new SingleProductDTO
            {
                SingleProductId = singleProduct.SingleProductId,
                SingleProductName = singleProduct.SingleProductName,
                SingleProductSerialNumber = singleProduct.SingleProductNumber,
                SingleProductActive = singleProduct.SingleProductActive,
                MainProduct = singleProduct.Product.Productname
            }
            );


            response.StatusCode = 200;

            return response;
        }

        public async Task<bool> ProductSerialNumberExists(string serialNumber)
        {
            var result = await _context.SingleProducts.AnyAsync(sp => sp.SingleProductNumber == serialNumber);

            if (!result)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> SingleProductExistsAsync(int productId)
        {
            var product = await _context.SingleProducts.FindAsync(productId);

            if (product == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<APIResponse<string>> updateSingleProduct(int productId, UpdateSingleProductDTO product)
        {
            var response = new APIResponse<string>();

            var existingSingleProduct = await _context.SingleProducts.FindAsync(productId);


            if (existingSingleProduct == null)
            {
                response.Errors.Add(new ErrorDetail { Field = "ProductId", Error = "SingleProduct nicht gefunden!" });
                response.StatusCode = 404;
                return response;
            }

            if (!string.IsNullOrWhiteSpace(product.SingleProductSerialNumber) && await _context.SingleProducts.AnyAsync(p => p.SingleProductNumber.ToLower() == product.SingleProductSerialNumber.ToLower().Trim() && p.SingleProductId != productId))
            {
                response.Errors.Add(new ErrorDetail { Field = "SingleProductSerialNumber", Error = "Die Produkt-Serien-Nr. existiert bereits!" });
            }

            if (product.ProductId.HasValue && !await _productService.ProductExistsAsync((int)product.ProductId))
            {
                response.Errors.Add(new ErrorDetail { Field = "ProductId", Error = "Das Produkt mit dieser Id gibt es nicht!" });
            }


            if (response.Errors.Any())
            {
                response.StatusCode = 400;
                return response;
            }


            existingSingleProduct!.SingleProductName = product.SingleProductName ?? existingSingleProduct.SingleProductName;
            existingSingleProduct!.SingleProductNumber = product.SingleProductSerialNumber ?? existingSingleProduct.SingleProductNumber;
            existingSingleProduct!.SingleProductActive = product.SingleProductActive ?? existingSingleProduct.SingleProductActive;
            existingSingleProduct!.ProductId = product.ProductId ?? existingSingleProduct.ProductId;

            _context.SingleProducts.Update(existingSingleProduct);
            await _context.SaveChangesAsync();

            response.Data = "SingleProduct erfolgreich aktualisiert!";
            response.StatusCode = 200;

            return response;
        }

        public async Task<List<SingleProduct>> GetSingleproductsForProduct(int productId)
        {
            return await _context.SingleProducts
            .Where(singleProduct => singleProduct.ProductId == productId).ToListAsync();
        }

        public async Task<List<SingleProduct>> GetRentedSingleproducts(int productId)
        {
            return await _context.Rentals
                            .Include(rentals => rentals.SingleProducts)
                            .Where(rental =>
                            rental.SingleProducts.Any(sp => sp.ProductId == productId
                            && rental.Status != 3
                            && rental.Status != 5
                            && rental.Status != 6
                            ))
                            .SelectMany(rental => rental.SingleProducts)
                            .ToListAsync();

        }

        public async Task<List<SingleProduct>> GetRentableSingleproducts(int productId)
        {
            var singleProducts = await GetSingleproductsForProduct(productId);

            var rentedSingleProducts = await GetRentedSingleproducts(productId);

            var inactiveSingleProducts = singleProducts.Where(sp => sp.SingleProductActive == false).ToList();


            if (rentedSingleProducts.Any())
            {
                foreach (var sp in rentedSingleProducts)
                {
                    singleProducts.Remove(sp);
                }
            }

            if (inactiveSingleProducts.Any())
            {
                foreach (var sp in inactiveSingleProducts)
                {
                    singleProducts.Remove(sp);
                }
            }

            return singleProducts;

        }

        public async Task<List<SingleProduct>> GetNeededSingleProducts(int productId, int amount)
        {
            var sp = await GetRentableSingleproducts(productId);
            var needed = sp.Take(amount).ToList();
            return needed;
        }
    }
}
