using MaReSy2_Api.Models.DTO;
using MaReSy2_Api.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MaReSy2_Api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
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
        public async Task<ActionResult<ProductDTO>> createProduct(string Productname, string Productdescription, int Productactive, int Productamount)
        {
            var result  = await _productService.AddNewProduct(Productname, Productdescription, Productactive, Productamount);

            var (createdProduct, errors ) = result;

            if (errors != null && errors.Any())
            {
                return BadRequest(new { Errors = errors });
            }

            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductId }, createdProduct);

        }

       

       
    }
}
