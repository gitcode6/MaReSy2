using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IImageUploadService _imageUploadService;
        private readonly MaReSyDbContext _maReSyDbContext;

        public ProductController(IProductService productService, MaReSyDbContext maReSyDbContext, IImageUploadService imageUploadService)
        {
            _productService = productService;
            _maReSyDbContext = maReSyDbContext;
            _imageUploadService = imageUploadService;
        }


        // GET: api/<ProductController>
        [HttpGet("")]
        public async Task<ActionResult<List<ProductDTO>>> Get()
        {
            var products = await _productService.GetProductsAsync();

            return Ok(products);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);


            //TODO: Produktvalidierung in den Service verschieben??
            if (product == null)
            {
                var errors = IdentityResult.Failed(new IdentityError() { Description = "Produkt wurde nicht gefunden!" });
                return BadRequest(errors);
            }
            else
            {
                return Ok(product);
            }
        }

        // POST api/<ProductController>
        [HttpPost("")]
        public async Task<ActionResult<ProductDTO>> CreateProduct(CreateProductDTO product)
        {
            Validator.ValidateObject(product, new ValidationContext(product), validateAllProperties: true);

            if (ModelState.IsValid)
            {
                var result = await _productService.AddNewProduct(product);

                if (result.Contains(IdentityResult.Success))
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }

            return BadRequest();




            //var (createdProduct, errors) = result;

            //if (errors != null && errors.Any())
            //{
            //    return BadRequest(new { Errors = errors });
            //}

            //return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);

        }

        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var product = await _maReSyDbContext.Products.FindAsync(id);

            if (product == null)
            {
                return BadRequest(IdentityResult.Failed(new IdentityError() { Description = "Produkt wurde nicht gefunden!" }));

                //NotFound("Produkt nicht gefunden");
            }

            if (product.Productimage == null || product.Productimage.Length == 0)
            {
                return BadRequest(IdentityResult.Failed(new IdentityError() { Description = $"Kein Produktbild für Produkt (ID: {product.ProductId}) vorhanden." }));

            }

            var imageType = "image/jpeg";

            return File(product.Productimage, imageType);
        }

        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadProductImage(int id, [FromForm] IFormFile image)
        {
            try
            {
                var imageBytes = await _imageUploadService.ValidateAndProcessImageAsync(image);
                var product = await _maReSyDbContext.Products.FindAsync(id);

                if(product == null) return NotFound("Das Produkt wurde nicht gefunden");

                product.Productimage = imageBytes;
                _maReSyDbContext.Products.Update(product);
                await _maReSyDbContext.SaveChangesAsync();

                return Ok("Bild erfolgreich hochgeladen!");
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch
            {
                return StatusCode(500, "Ein unerwarteter Fehler ist aufgetreten.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deactivateProduct(int id)
        {
            var result = await _productService.deleteProductAsync(id);

            if (result == IdentityResult.Success)
            {
                return Ok(result);
            }

            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateProduct(int id, UpdateProductDTO product)
        {
            var result = await _productService.updateProduct(id, product);

            if (result.Contains(IdentityResult.Success))
            {
                return Ok(result);
            }

            else
            {
                return BadRequest(result);
            }
        }

        //[HttpPost("{id}/image")]
        //public async Task<IActionResult> UploadImage(int id)
        //{

        //}



    }
}
