using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.SingleProductDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Transactions;

namespace MaReSy2_Api.Services
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

        public async Task<List<IdentityResult>> AddNewSingleProduct(CreateSingleProductDTO singleProduct)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            if (string.IsNullOrWhiteSpace(singleProduct.SingleProductName))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Bitte gib einen Produktnamen an!" }));
            }

            if (string.IsNullOrWhiteSpace(singleProduct.SingleProductSerialNumber))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Bitte gib eine Produkt-Serien-Nr. ein!" }));
            }

            if (!await _productService.ProductExistsAsync(singleProduct.ProductId))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Das Produkt mit dieser Id gibt es nicht!" }));
            }

            if (await ProductSerialNumberExists(singleProduct.SingleProductSerialNumber.ToLower().Trim()))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Die Produkt-Serien-Nr. existiert bereits!" }));
            }

            if (errors.Any())
            {
                return errors;
            }

            var newSingleProduct = new SingleProduct
            {
                SingleProductName = singleProduct.SingleProductName,
                SingleProductNumber = singleProduct.SingleProductSerialNumber,
                SingleProductActive = singleProduct.SingleProductActive,
                ProductId = singleProduct.ProductId,
            };

            var result = await _context.SingleProducts.AddAsync(newSingleProduct);
            await _context.SaveChangesAsync();


            errors.Add(IdentityResult.Success);

            return errors;




        }

        public async Task<IdentityResult> deleteSingleProductAsync(int productId)
        {
            var singleProduct = await _context.SingleProducts.FindAsync(productId);


            if (singleProduct == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Singleproduct wurde nicht gefunden!" });
            }

            singleProduct.SingleProductActive = false;

            _context.SingleProducts.Update(singleProduct);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<SingleProductDTO?> GetSingleProductAsync(int productId)
        {
            var result = await SingleProductExistsAsync(productId);
            SingleProductDTO? singleProduct = null;

            if (result == true)
            {
                var product = await _context.SingleProducts.Include(x => x.Product).FirstAsync(x => x.SingleProductId == productId);
                singleProduct = new SingleProductDTO
                {
                    SingleProductId = product.SingleProductId,
                    SingleProductName = product.SingleProductName,
                    SingleProductActive = product.SingleProductActive,
                    SingleProductSerialNumber = product.SingleProductNumber,
                    MainProduct = product.Product.Productname
                };

            }

            return singleProduct;
        }

        public async Task<IEnumerable<SingleProductDTO>> GetSingleProductsAsync()
        {
            var SingleProducts = _context.SingleProducts
                .Include(sp => sp.Product);

            var SingleProductList = await SingleProducts.ToListAsync();

            return SingleProductList.Select(singleProduct => new SingleProductDTO
            {
                SingleProductId = singleProduct.SingleProductId,
                SingleProductName = singleProduct.SingleProductName,
                SingleProductSerialNumber = singleProduct.SingleProductNumber,
                SingleProductActive = singleProduct.SingleProductActive,
                MainProduct = singleProduct.Product.Productname
            }
                );
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

        public async Task<List<IdentityResult>> updateSingleProduct(int productId, UpdateSingleProductDTO product)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            var existingSingleProduct = await _context.SingleProducts.FindAsync(productId);


            if (existingSingleProduct == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Singleproduct nicht gefunden!" }));

                return errors;
            }


            //TODO: DARF EIN SINGLEPRODUKT MIT DEM GLEICHEN NAMEN EXISITIEREN?
            if (!string.IsNullOrWhiteSpace(product.SingleProductName) && await _context.SingleProducts.AnyAsync(p => p.SingleProductName.ToLower() == product.SingleProductName.ToLower().Trim() && p.ProductId != productId))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Der Produktname ist bereits vorhanden." }));
            }

            if (!string.IsNullOrWhiteSpace(product.SingleProductSerialNumber) && await _context.SingleProducts.AnyAsync(p => p.SingleProductNumber.ToLower() == product.SingleProductSerialNumber.ToLower().Trim() && p.SingleProductId != productId))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Die Produkt-Serien-Nr. existiert bereits!" }));
            }

            if (product.ProductId.HasValue && !await _productService.ProductExistsAsync((int)product.ProductId))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Das Produkt mit dieser Id gibt es nicht!" }));
            }


            if (errors.Count > 0)
            {
                return errors;
            }


            existingSingleProduct!.SingleProductName = product.SingleProductName ?? existingSingleProduct.SingleProductName;
            existingSingleProduct!.SingleProductNumber = product.SingleProductSerialNumber ?? existingSingleProduct.SingleProductNumber;
            existingSingleProduct!.SingleProductActive = product.SingleProductActive ?? existingSingleProduct.SingleProductActive;
            existingSingleProduct!.ProductId = product.ProductId ?? existingSingleProduct.ProductId;

            _context.SingleProducts.Update(existingSingleProduct);
            await _context.SaveChangesAsync();

            errors.Add(IdentityResult.Success);
            return errors;
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
            var singleProducts = await GetSingleproductsForProduct (productId);

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
    }
}
