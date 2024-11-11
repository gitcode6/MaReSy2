using MaReSy2_Api.Models.DTO.ProductDTO;
using MaReSy2_Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly MaReSyDbContext _maReSyDbContext;

        public ProductController(IProductService productService, MaReSyDbContext maReSyDbContext)
        {
            _productService = productService;
            _maReSyDbContext = maReSyDbContext;
        }


        // GET: api/<ProductController>
        [HttpGet("")]
        public async Task<ActionResult<List<ProductDTO>>> Get()
        {
            var products =  await _productService.GetProductsAsync();

            return Ok(products);
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDTO>> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if(product == null)
            {
                return BadRequest();
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
            var result  = await _productService.AddNewProduct(product);

            var (createdProduct, errors ) = result;

            if (errors != null && errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);

        }

        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetProductImage(int id)
        {
            var product = await _maReSyDbContext.Products.FindAsync(id);

            if(product == null)
            {
                return NotFound("Produkt nicht gefunden");
            }

            if(product.Productimage == null || product.Productimage.Length == 0)
            {
                return NotFound($"Kein Produktbild für Produkt (ID: {product.ProductId}) vorhanden.");
            }

            var imageType = "image/jpeg";

            return File(product.Productimage, imageType);
        }

        //[HttpPost("{id}/image")]
        //public async Task<IActionResult> UploadImage(int id)
        //{
            
        //}
       

       
    }
}
