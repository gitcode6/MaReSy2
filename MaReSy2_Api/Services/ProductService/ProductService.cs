using System.Collections.Generic;
using System.ComponentModel;
using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.RentalDTO;
using MaReSy2_Api.Models.DTO.UserDTO;
using MaReSy2_Api.Services.ProductService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MaReSy2_Api.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly MaReSyDbContext _context;

        public ProductService(MaReSyDbContext context)
        {
            _context = context;
        }

        public async Task<APIResponse<bool>> AddNewProduct(CreateProductDTO product)
        {

            var result = new APIResponse<bool>();


            if (string.IsNullOrWhiteSpace(product.Productname))
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "Productname",
                    Error = "Der Productname ist erforderlich!",
                });
            }

            //if (!int.IsPositive(product.Productamount))
            //{
            //    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Productamount muss positiv (>= 0) sein." }));
            //}

            //if ((product.Productactive != true) && (product.Productactive != false))
            //{
            //    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Productactive muss entweder true oder false sein." }));
            //}

            if (await ProductExistsAsync(product.Productname))
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "Productname",
                    Error = "Der Produktname ist bereits vergeben",
                });
            }


            if (result.Errors.Any())
            {
                result.StatusCode = 400;
                result.Data = false;
                return result;
            }

            var newProduct = new Product
            {
                Productname = product.Productname,
                Productdescription = product.Productdescription,
                ProductActive = product.Productactive,
            };

            /*var result = */
            await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            result.Message = "Produkt wurde erfolgreich erstellt!";
            result.StatusCode = 200;
            result.Data = true;
            //var createdProductDto = new ProductDTO
            //{
            //    ProductId = newProduct.ProductId,
            //    Productname = newProduct.Productname,
            //    Productdescription = newProduct.Productdescription,
            //    Productactive = newProduct.Productactive,
            //    Productamount = newProduct.Productamount
            //};

            return result;

        }

        public async Task<APIResponse<bool>> deleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            var result = new APIResponse<bool>();

            if (product == null)
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "productId",
                    Error = "Produkt nicht gefunden"
                });

                result.StatusCode = 404;
                return result;
            }

            product.ProductActive = false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            result.Message = "Produkt erfolgreich deaktiviert";
            result.StatusCode = 200;
            result.Data = true;
            return result;
        }

        public async Task<APIResponse<ProductDTO>> GetProductByIdAsync(int productId)
        {
            var result = new APIResponse<ProductDTO?>();

            var findSuccess = await ProductExistsAsync(productId);

            if (findSuccess == false)
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "productId",
                    Error = "Produkt nicht gefunden!"
                });
                result.StatusCode = 404;
                result.Message = "Das Produkt wurde nicht gefunden.";
                return result;
            }


            var product_fromDb = await _context.Products.FirstAsync(p => p.ProductId == productId);


            result.Data = new ProductDTO
            {
                ProductId = product_fromDb.ProductId,
                Productname = product_fromDb.Productname,
                Productdescription = product_fromDb.Productdescription,
                Productactive = product_fromDb.ProductActive,
                ProductimageLink = product_fromDb.Productimage != null && product_fromDb.Productimage.Length != 0 ? $"/api/products/{product_fromDb.ProductId}/image" : null,
            };



            result.StatusCode = 200;
            return result;

        }

        public async Task<APIResponse<IEnumerable<ProductDTO>>>? GetProductsAsync()
        {
            var result = new APIResponse<IEnumerable<ProductDTO>>();

            var products = _context.Products;

            var productList = await products.ToListAsync();

            result.Data = productList.Select(product => new ProductDTO
            {
                ProductId = product.ProductId,
                Productname = product.Productname,
                Productdescription = product.Productdescription,
                Productactive = product.ProductActive,
                ProductimageLink = product.Productimage != null && product.Productimage.Length != 0 ? $"/api/products/{product.ProductId}/image" : null,
            });

            result.StatusCode = 200;


            return result;
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            return product != null;


        }

        public async Task<bool> ProductExistsAsync(string productname)
        {
            var product = await _context.Products.AnyAsync(p => p.Productname == productname);

            if (!product)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task<APIResponse<bool>> updateProduct(int id, UpdateProductDTO product)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            var result = new APIResponse<bool>();

            var existingProduct = await _context.Products.FindAsync(id);


            if (existingProduct == null)
            {
                //errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Produkt nicht gefunden!" }));

                result.Errors.Add(new ErrorDetail
                {
                    Field = "ProductId",
                    Error = "Produkt wurde nicht gefunden!",
                });

                result.StatusCode = 404;


                return result;
            }

            if (!string.IsNullOrEmpty(product.Productname) && await _context.Products.AnyAsync(p => p.Productname.ToLower() == product.Productname.ToLower() && p.ProductId != id))
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "Productname",
                    Error = "Der Produktname ist bereits vorhanden"
                });

                //errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Der Produktname ist bereits vorhanden." }));
            }

            //if (product.Productamount != null && !int.IsPositive(product.Productamount.Value))
            //{
            //    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Die Produktanzahl muss positiv sein" }));
            //}

            if (result.Errors.Any())
            {
                result.StatusCode = 400;
                return result;
            }


            existingProduct!.Productname = product.Productname ?? existingProduct.Productname;
            existingProduct!.Productdescription = product.Productdescription ?? existingProduct.Productdescription;
            existingProduct!.ProductActive = product.Productactive ?? existingProduct.ProductActive;

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            result.Data = true;
            result.StatusCode = 200;
            result.Message = "Produkt erfolgreich aktualisiert";

            return result;


        }


        public async Task<List<ProductWithAmount>> GetProductsForSet(int setId)
        {
            return await _context.ProductsSets
.Where(ps => ps.SetId == setId)
.Select(ps => new ProductWithAmount
{
    product = ps.Product,
    productAmount = ps.SingleProductAmount
})
.ToListAsync();
        }
        public async Task<bool> SetContainsInactiveProduct(int setId)
        {
            var productsWithAmount = await GetProductsForSet(setId);
            return productsWithAmount.Any(x => x.product.ProductActive == false);
        }


    }
}
