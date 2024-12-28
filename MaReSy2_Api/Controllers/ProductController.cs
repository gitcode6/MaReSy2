using Azure;
using MaReSy2_Api.Models;
using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Services.ImageService;
using MaReSy2_Api.Services.ProductService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    [Authorize]
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
        public async Task<ActionResult<APIResponse<IEnumerable<ProductDTO>>>> Get()
        {
            var result = await _productService.GetProductsAsync();

            return helperMethod.ToActionResult(result, this);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<ProductDTO>>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            return helperMethod.ToActionResult(product, this);
        }

        // POST api/<ProductController>
        [HttpPost("")]
        public async Task<ActionResult<APIResponse<bool>>> CreateProduct(CreateProductDTO product)
        {
            var result = await _productService.AddNewProduct(product);

            return helperMethod.ToActionResultBasic(result, this);
        }

        [HttpGet("{id}/image")]
        public async Task<ActionResult<APIResponse<byte[]>>> GetProductImage(int id)
        {

            var result = new APIResponse<byte[]>();

            var product = await _maReSyDbContext.Products.FindAsync(id);

            if (product == null)
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "ProductId",
                    Error = "Produkt wurde nicht gefunden!"
                });
                result.StatusCode = 404;
                return helperMethod.ToActionResult(result, this);
            }

            if (product.Productimage == null || product.Productimage.Length == 0)
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "ProductImage",
                    Error = $"Kein Produktbild für Produkt (ID: {product.ProductId}) vorhanden."
                });
                result.StatusCode = 400;
                return helperMethod.ToActionResult(result, this);

            }

            var imageType = "image/jpeg";

            return File(product.Productimage, imageType);
        }

        [HttpPost("{id}/upload-image")]
        public async Task<ActionResult<APIResponse<string>>> UploadProductImage(int id, IFormFile image)
        {
            var result = new APIResponse<string>();


            try
            {
                var imageBytes = await _imageUploadService.ValidateAndProcessImageAsync(image);
                var product = await _maReSyDbContext.Products.FindAsync(id);

                if (product == null)
                {
                    result.Errors.Add(new ErrorDetail
                    {
                        Field = "ProductId",
                        Error = "Das Produkt wurde nicht gefunden."
                    });
                    result.StatusCode = 404;
                    return helperMethod.ToActionResult(result, this);
                }
                product.Productimage = imageBytes;
                _maReSyDbContext.Products.Update(product);
                await _maReSyDbContext.SaveChangesAsync();

                result.Data = "Bild erfolgreich hochgeladen!";
                result.StatusCode = 200;
                return helperMethod.ToActionResultBasic(result, this);
            }
            catch (ArgumentException ex)
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "Image",
                    Error = ex.Message
                });
                result.StatusCode = 400;
                return helperMethod.ToActionResult(result, this);
            }
            catch
            {
                result.Errors.Add(new ErrorDetail
                {
                    Field = "General",
                    Error = "Ein unerwarteter Fehler ist aufgetreten."
                });
                result.StatusCode = 500;
                return helperMethod.ToActionResult(result, this);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deactivateProduct(int id)
        {
            var result = await _productService.deleteProductAsync(id);

            return StatusCode(result.StatusCode ?? 400, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateProduct(int id, UpdateProductDTO product)
        {
            var result = await _productService.updateProduct(id, product);

            return StatusCode(result.StatusCode ?? 400, result);
        }



    }
}
