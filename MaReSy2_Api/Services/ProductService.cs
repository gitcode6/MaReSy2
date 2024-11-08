using MaReSy2_Api.Models.DTO.ProductDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MaReSy2_Api.Services
{
    public class ProductService : IProductService
    {
        private readonly MaReSyDbContext _context;

        public ProductService(MaReSyDbContext context)
        {
            _context = context;
        }

        public async Task<(ProductDTO? CreatedProduct, List<string>? errors)> AddNewProduct(CreateProductDTO product)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(product.Productname))
            {
                errors.Add("Der Productname ist erforderlich!");
            }

            if (!int.IsPositive(product.Productamount))
            {
                errors.Add("Productamount muss positiv (>= 0) sein.");
            }

            if((product.Productactive != true) &&  (product.Productactive != false))
            {
                errors.Add("Productactive muss entweder true oder false sein.");
            }


            if (errors.Any())
            {
                return (null, errors);
            }

            var newProduct = new Product
            {
                Productname = product.Productname,
                Productdescription = product.Productdescription,
                Productactive = product.Productactive,
                Productamount = product.Productamount,
            };

            var result = await _context.Products.AddAsync(newProduct);
            await _context.SaveChangesAsync();

            var createdProductDto = new ProductDTO
            {
                ProductId = newProduct.ProductId,
                Productname = newProduct.Productname,
                Productdescription = newProduct.Productdescription,
                Productactive = newProduct.Productactive,
                Productamount = newProduct.Productamount
            };

            return (createdProductDto, null);

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
                        Productactive = product_fromDb.Productactive,
                        Productamount = product_fromDb.Productamount,
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
                Productactive = product.Productactive,
                Productamount = product.Productamount,
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
    }
}
