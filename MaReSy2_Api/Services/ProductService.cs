using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Models.DTO.UserDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MaReSy2_Api.Services
{
    public class ProductService : IProductService
    {
        private readonly MaReSyDbContext _context;

        public ProductService(MaReSyDbContext context)
        {
            _context = context;
        }

        public async Task<List<IdentityResult>> AddNewProduct(CreateProductDTO product)
        {
            List<IdentityResult> errors = new List<IdentityResult>();


            if (string.IsNullOrWhiteSpace(product.Productname))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Der Productname ist erforderlich!" }));
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
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Der Produktname ist bereits vorhanden." }));
            }


            if (errors.Any())
            {
                return errors;
            }

            var newProduct = new Product
            {
                Productname = product.Productname,
                Productdescription = product.Productdescription,
                ProductActive = product.Productactive,
            };

            var result = await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            errors.Add(IdentityResult.Success);

            //var createdProductDto = new ProductDTO
            //{
            //    ProductId = newProduct.ProductId,
            //    Productname = newProduct.Productname,
            //    Productdescription = newProduct.Productdescription,
            //    Productactive = newProduct.Productactive,
            //    Productamount = newProduct.Productamount
            //};

            return errors;

        }

        public async Task<IdentityResult> deleteProductAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return IdentityResult.Failed(new IdentityError() { Description = "Produkt wurde nicht gefunden!" });
            }

            product.ProductActive = false;

            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int productId)
        {
            var result = await ProductExistsAsync(productId);
            ProductDTO? product = null;

            if (result == true)
            {
                var product_fromDb = await _context.Products.FirstAsync(p => p.ProductId == productId);

                if (product_fromDb != null)
                {
                    product = new ProductDTO
                    {
                        ProductId = product_fromDb.ProductId,
                        Productname = product_fromDb.Productname,
                        Productdescription = product_fromDb.Productdescription,
                        Productactive = product_fromDb.ProductActive
                    };
                }


            }

            return product;
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsAsync()
        {
            var products = _context.Products;

            var productList = await products.ToListAsync();

            return productList.Select(product => new ProductDTO
            {
                ProductId = product.ProductId,
                Productname = product.Productname,
                Productdescription = product.Productdescription,
                Productactive = product.ProductActive,
                ProductimageLink = product.Productimage != null && product.Productimage.Length != 0 ? $"/api/products/{product.ProductId}/image" : null,
            });
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product == null)
            {
                return false;
            }
            else
            {
                return true;
            }


        }

        public async Task<bool> ProductExistsAsync(string productname)
        {
            var product = await _context.Products.AnyAsync(p=>p.Productname == productname);

            if (!product)
            {
                return false;
            }
            else
            {
                return true;
            }


        }




        public async Task<List<IdentityResult>> updateProduct(int id, UpdateProductDTO product)
        {
            List<IdentityResult> errors = new List<IdentityResult>();

            var existingProduct = await _context.Products.FindAsync(id);


            if (existingProduct == null)
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Produkt nicht gefunden!" }));

                return errors;
            }

            if (!string.IsNullOrEmpty(product.Productname) && await _context.Products.AnyAsync(p => p.Productname.ToLower() == product.Productname.ToLower() && p.ProductId != id))
            {
                errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Der Produktname ist bereits vorhanden." }));
            }

            //if (product.Productamount != null && !int.IsPositive(product.Productamount.Value))
            //{
            //    errors.Add(IdentityResult.Failed(new IdentityError() { Description = "Die Produktanzahl muss positiv sein" }));
            //}

            if (errors.Count > 0)
            {
                return errors;
            }


            existingProduct!.Productname = product.Productname ?? existingProduct.Productname;
            existingProduct!.Productdescription = product.Productdescription ?? existingProduct.Productdescription;
            existingProduct!.ProductActive = product.Productactive ?? existingProduct.ProductActive;

            _context.Products.Update(existingProduct);
            await _context.SaveChangesAsync();

            errors.Add(IdentityResult.Success);
            return errors;


        }
    }
}
