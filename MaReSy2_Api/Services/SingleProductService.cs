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

            if (await ProductSerialNumberExists(singleProduct.SingleProductSerialNumber))
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
    }
}
